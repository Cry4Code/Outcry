using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

// GCP 및 Google Sheets API 사용
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;

public class GoogleSheetsToJsonConverter : EditorWindow
{
    private string jsonOutputPath = "Assets/Resources/GeneratedJson";
    private string scriptOutputPath = "Assets/02. Scripts/GeneratedData";
    private string schemaOutputPath => jsonOutputPath;

    private string spreadSheetId = "여기에_구글_스프레드시트_ID를_입력하세요";
    private string credentialPath = "Assets/Editor/Credentials/your-credential-file-name.json";

    private const string SpreadSheetIdKey = "GcpConverter_SpreadsheetId";
    private const string CredentialPathKey = "GcpConverter_CredentialPath";

    private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

    [MenuItem("Tools/Data Converter/Google Sheets to JSON Converter")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetsToJsonConverter>("Google Sheets to JSON");
    }

    private void OnEnable()
    {
        spreadSheetId = EditorPrefs.GetString(SpreadSheetIdKey, spreadSheetId);
        credentialPath = EditorPrefs.GetString(CredentialPathKey, credentialPath);
    }

    // 2단계 버튼 워크플로우 UI
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Google Sheet to JSON Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Google Spreadsheet ID", EditorStyles.boldLabel);
        spreadSheetId = EditorGUILayout.TextField(spreadSheetId);
        EditorGUILayout.LabelField("Service Account Credential Path", EditorStyles.boldLabel);
        credentialPath = EditorGUILayout.TextField(credentialPath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output Folder (C# Scripts)", EditorStyles.boldLabel);
        scriptOutputPath = EditorGUILayout.TextField(scriptOutputPath);
        EditorGUILayout.LabelField("Output Folder (JSON & Schema)", EditorStyles.boldLabel);
        jsonOutputPath = EditorGUILayout.TextField(jsonOutputPath);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 버튼을 오른쪽으로 정렬하기 위한 빈 공간
        if (GUILayout.Button("Clear Saved Settings", GUILayout.Width(150)))
        {
            // 사용자의 실수를 방지하기 위한 확인창
            if (EditorUtility.DisplayDialog("Clear Saved Settings",
                "Are you sure you want to clear the saved Spreadsheet ID and Credential Path?",
                "Yes", "Cancel"))
            {
                // EditorPrefs에 저장된 키 삭제
                EditorPrefs.DeleteKey(SpreadSheetIdKey);
                EditorPrefs.DeleteKey(CredentialPathKey);

                // UI에 표시되는 변수를 초기값으로 되돌림
                spreadSheetId = "여기에_구글_스프레드시트_ID를_입력하세요";
                credentialPath = "Assets/Editor/Credentials/your-credential-file-name.json";

                // UI 즉시 갱신 -> Repaint() 호출
                Repaint();
                Debug.Log("Saved settings have been cleared.");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        // Step 1 버튼(C# 클래스 및 스키마 생성)
        EditorGUILayout.HelpBox("Step 1: Fetches ALL sheets and generates C# class files and schema files. Run this first, or when the sheet structure has changed.", MessageType.Info);
        if (GUILayout.Button("Step 1: Generate C# Classes & Schemas"))
        {
            GenerateAllClasses();
        }

        EditorGUILayout.Space(10);

        // Step 2 버튼(시트 데이터를 JSON으로 변환)
        EditorGUILayout.HelpBox("Step 2: Converts sheets to JSON files. It will check if the structure has changed. Run this after Step 1 and re-compilation is complete.", MessageType.Info);
        if (GUILayout.Button("Step 2: Convert Sheets to JSON"))
        {
            ConvertAllToJson();
        }
    }

    #region 기능 메서드
    // Step 1 버튼 로직
    private void GenerateAllClasses()
    {
        if (!ValidateInputs()) return;
        EditorPrefs.SetString(SpreadSheetIdKey, spreadSheetId);
        EditorPrefs.SetString(CredentialPathKey, credentialPath);

        var service = Authenticate();
        if (service == null) return;

        var tables = FetchAllTables(service);
        if (tables == null) return;

        foreach (var pair in tables)
        {
            GenerateCSharpClassFile(pair.Value, pair.Key, scriptOutputPath, schemaOutputPath);
        }

        AssetDatabase.Refresh();
        Debug.Log("Step 1 Complete: C# classes and schemas generated. Unity will now recompile. Please run Step 2 after recompilation.");
        EditorUtility.DisplayDialog("Step 1 Complete", "C# classes generated. Please run Step 2 after Unity finishes recompiling.", "OK");
    }

    // Step 2 버튼 로직
    private void ConvertAllToJson()
    {
        if (!ValidateInputs())
        {
            return;
        }

        var service = Authenticate();
        if (service == null)
        {
            return;
        }

        var sheetNames = GetSheetNames(service);
        if (sheetNames == null)
        {
            return;
        }

        bool allSucceeded = true;
        foreach (string sheetName in sheetNames)
        {
            // 스키마 검사를 먼저 수행
            if (!CheckSchema(service, sheetName))
            {
                allSucceeded = false;
                continue; // 스키마가 다르면 다음 시트로 건너뜀
            }

            // 스키마가 일치하면 데이터 변환 진행
            var values = GetSheetValues(service, sheetName);
            if (values != null && values.Count > 0)
            {
                DataTable table = ParseValueRangeToDataTable(values, sheetName);
                if (table != null)
                {
                    if (!CheckIfClassExist(sheetName))
                    {
                        Debug.LogError($"Class '{sheetName}' could not be found. Please run Step 1 and wait for recompile.");
                        allSucceeded = false;
                        continue;
                    }
                    ConvertSheetToJson(table, sheetName, jsonOutputPath);
                }
            }
        }

        AssetDatabase.Refresh();
        if (allSucceeded)
        {
            Debug.Log("Step 2 Complete: All sheets successfully converted to JSON.");
            EditorUtility.DisplayDialog("Step 2 Complete", "All sheets successfully converted to JSON.", "OK");
        }
        else
        {
            Debug.LogError("Step 2 Finished: Some sheets were not converted. Please check the console for errors.");
            EditorUtility.DisplayDialog("Step 2 Finished", "Some sheets were not converted. Please check the console for errors and run Step 1 if necessary.", "OK");
        }
    }
    #endregion

    #region 유틸리티 메서드
    // Google Sheets API 인증
    private SheetsService Authenticate()
    {
        // 로컬 경로에 인증 키 파일(.json) 존재하는지 확인
        if (!File.Exists(credentialPath))
        {
            Debug.LogError($"Credential file not found at: {credentialPath}");
            return null;
        }

        GoogleCredential credential;
        // using 키워드는 코드 블록이 끝나면 파일 스트림을 자동으로 닫아주어 리소스 관리에 안전
        using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            // 파일 스트림에서 서비스 계정의 인증 정보를 읽어옴
            // CreateScoped(Scopes)는 이 인증 정보로 어떤 API 권한을 사용할지 지정(읽기 전용)
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        // 인증 정보를 사용하여 Sheets API와 통신할 서비스 객체를 생성하고 반환
        return new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential, // 인증 정보 전달
            ApplicationName = "Unity Data Converter", // API 사용 로그에 표시될 애플리케이션 이름
        });
    }

    private bool ValidateInputs()
    {
        // 스프레드시트 ID가 비어있거나 초기값 그대로인지 확인
        if (string.IsNullOrEmpty(spreadSheetId) || spreadSheetId.Contains("ID를_입력"))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a valid Google Spreadsheet ID.", "OK");
            return false;
        }

        // C# 클래스와 JSON 파일이 저장될 폴더가 없다면 자동으로 생성
        Directory.CreateDirectory(scriptOutputPath);
        Directory.CreateDirectory(jsonOutputPath);

        return true;
    }

    private List<string> GetSheetNames(SheetsService service)
    {
        try
        {
            // Spreadsheets.Get API 호출하여 스프레드시트의 전체 메타데이터 요청
            var spreadsheet = service.Spreadsheets.Get(spreadSheetId).Execute();
            // 받아온 메타데이터에서 Sheets 목록을 꺼내 각 시트(s)의 제목(Properties.Title)만 추출하여 리스트로 만듦
            var sheetNames = spreadsheet.Sheets.Select(s => s.Properties.Title).ToList();
            Debug.Log($"Found {sheetNames.Count} sheets: {string.Join(", ", sheetNames)}");
            return sheetNames;
        }
        catch (Exception e)
        {
            // ID가 틀리거나 권한이 없는 등 API 호출에 실패하면 에러 출력
            Debug.LogError($"Failed to get spreadsheet metadata. Check your Spreadsheet ID and permissions. Error: {e.Message}");
            return null;
        }
    }

    // 특정 시트의 모든 값을 가져오는 메서드
    private IList<IList<object>> GetSheetValues(SheetsService service, string sheetName)
    {
        try
        {
            // 데이터 가져올 범위 지정
            string range = $"{sheetName}!A1:Z";
            // Spreadsheets.Values.Get API 호출하여 지정된 범위의 셀 값들 요청
            var request = service.Spreadsheets.Values.Get(spreadSheetId, range);
            var response = request.Execute();
            // 응답 객체에서 실제 데이터 목록(Values) 반환
            return response.Values;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get data from sheet '{sheetName}'. Error: {e.Message}");
            return null;
        }
    }

    // 모든 시트 데이터를 가져와 DataTable 딕셔너리로 만드는 메서드
    private Dictionary<string, DataTable> FetchAllTables(SheetsService service)
    {
        // 전체 시트 목록을 가져옴
        var sheetNames = GetSheetNames(service);
        if (sheetNames == null)
        {
            return null;
        }

        var tables = new Dictionary<string, DataTable>();
        foreach (string sheetName in sheetNames)
        {
            // 해당 시트 데이터 가져옴
            var values = GetSheetValues(service, sheetName);
            if (values != null && values.Count > 0)
            {
                // 데이터를 DataTable로 변환
                DataTable table = ParseValueRangeToDataTable(values, sheetName);
                if (table != null)
                {
                    tables.Add(sheetName, table);
                }
            }
            else
            {
                Debug.LogWarning($"No data found in sheet: {sheetName}.");
            }
        }

        return tables;
    }

    // Google Sheets API의 응답(ValueRange)을 DataTable로 변환
    private DataTable ParseValueRangeToDataTable(IList<IList<object>> values, string tableName)
    {
        // 헤더가 3행에 있으므로 최소 3줄이 있어야 유효한 데이터로 간주
        if (values.Count < 3)
        {
            return null;
        }

        DataTable table = new DataTable(tableName);

        // 헤더(세번째 행)를 기준으로 컬럼 생성. values[0]이 1행, values[2]가 3행에 해당
        var headers = values[2];
        foreach (var header in headers)
        {
            table.Columns.Add(header.ToString());
        }

        // 데이터 행은 4행부터 추가
        for (int i = 3; i < values.Count; i++)
        {
            var rowData = values[i];
            DataRow row = table.NewRow();
            for (int j = 0; j < headers.Count; j++)
            {
                if (j < rowData.Count)
                {
                    row[j] = rowData[j];
                }
                else
                {
                    row[j] = ""; // 빈 셀 처리
                }
            }
            table.Rows.Add(row);
        }

        // C# 클래스 생성을 위해 DataTable의 첫 행을 헤더 정보로 재구성
        // 헬퍼 메서드들은 헤더가 항상 첫 번째 행에 있다고 가정
        DataRow headerRow = table.NewRow();
        for (int i = 0; i < table.Columns.Count; i++)
        {
            headerRow[i] = table.Columns[i].ColumnName;
        }
        table.Rows.InsertAt(headerRow, 0);

        return table;
    }

    // 시트 구조(스키마)가 변경되었는지 검사
    private bool CheckSchema(SheetsService service, string sheetName)
    {
        string schemaPath = Path.Combine(jsonOutputPath, $"{sheetName}.schema.json");
        if (!File.Exists(schemaPath))
        {
            Debug.LogError($"[Schema Check Failed] Schema file for '{sheetName}' not found. Please run Step 1 first.");
            return false;
        }

        // 구글 시트에서 현재 최신 데이터를 가져옴
        var values = GetSheetValues(service, sheetName);
        // 시트가 비어있거나 헤더 행(3행)이 없는 경우 확인
        // 이런 경우 구조 비교 자체가 무의미 -> 경고만 남기고 검사 통과(true)
        if (values == null || values.Count < 3)
        {
            Debug.LogWarning($"[Schema Check] Sheet '{sheetName}' has less than 3 rows. Cannot check schema.");
            return true; // 빈 시트로 간주하고 통과
        }

        // 현재 구글 시트의 헤더(3행, 인덱스는 2)를 List<string> 형태로 추출
        // Linq의 Select를 사용 -> 각 헤더 셀(h)을 문자열(h.ToString())로 변환
        var currentHeaders = values[2].Select(h => h.ToString()).ToList();

        // 로컬에 저장된 .schema.json 파일을 읽고 JsonUtility를 사용해 HeaderSchema 객체로 변환
        // savedSchema.headers에 저장된 헤더 목록 사용 가능
        var savedSchema = JsonUtility.FromJson<HeaderSchema>(File.ReadAllText(schemaPath));

        // 두 헤더 리스트 비교(SequenceEqual은 두 리스트의 내용과 순서까지 모두 동일해야 true 반환)
        if (!currentHeaders.SequenceEqual(savedSchema.headers))
        {
            // 두 헤더가 일치하지 않으면 데이터 테이블 구조 변경됨
            // JSON 변환 시 데이터가 깨질 수 있으므로 명확한 에러 메시지 출력
            // false 반환하여 변환 작업 중단
            Debug.LogError($"[Schema Check Failed] Structure of sheet '{sheetName}' has changed. Please run Step 1 to update the C# class and schema.");
            return false;
        }

        Debug.Log($"[Schema Check Success] Structure of sheet '{sheetName}' is up to date.");
        return true;
    }
    #endregion

    #region 헬퍼 메서드
    // 스키마 저장을 위한 간단한 클래스
    [Serializable]
    private class HeaderSchema
    {
        public List<string> headers;
    }

    private static bool CheckIfClassExist(string className)
    {
        return FindTypeInAssemblies(className) != null;
    }

    public static void GenerateCSharpClassFile(DataTable table, string className, string outPathCs, string outPathSchema)
    {
        if (table.Rows.Count == 0)
        {
            Debug.LogWarning($"Table '{className}' is empty. Skipping class generation.");
            return;
        }

        // headerRow: 변수명으로 사용할 3번째 행(DataTable에서는 첫 번째 행) 가져옴
        DataRow headerRow = table.Rows[0];
        // dataRowForTypeInference: 변수의 타입을 추론하기 위한 데이터 샘플(4번째 행) 가져옴
        // 만약 데이터가 하나도 없다면(Rows.Count가 1) 헤더 행을 샘플로 사용
        DataRow dataRowForTypeInference = table.Rows.Count > 1 ? table.Rows[1] : headerRow;

        // idFieldName: 첫 번째 열의 헤더 이름 가져옴
        string idFieldName = headerRow[0]?.ToString();
        // isCollection: 이 데이터가 여러 개가 있는 목록(Collection)인가 판단
        bool isCollection = !string.IsNullOrWhiteSpace(idFieldName) && (idFieldName.ToLower().Contains("id") || idFieldName.ToLower().Contains("key"));

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// Auto-generated by GoogleSheetsToJsonConverter");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("\n[Serializable]");
        sb.AppendLine($"public class {className}{(isCollection ? " : IData" : "")}");
        sb.AppendLine("{");

        if (isCollection)
        {
            // 목록형 데이터인데 첫 열이 정수(int)가 아니면 중단
            if (InferTypeOfValue(dataRowForTypeInference[0]) != typeof(int))
            {
                Debug.LogError($"'{className}' is detected as Collection Data, but its first column '{idFieldName}' is not an integer. Aborting class generation.");
                return;
            }
            sb.AppendLine($"    public int ID => {idFieldName};");
            sb.AppendLine();
        }

        // 변수(Field) 자동 생성
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string fieldName = headerRow[i]?.ToString();
            if (string.IsNullOrWhiteSpace(fieldName) || fieldName.Contains(" "))
            {
                Debug.LogWarning($"Invalid or empty field name in '{className}' at column {i + 1}. Skipping.");
                continue;
            }
            Type fieldType = InferTypeOfValue(dataRowForTypeInference[i]);
            sb.AppendLine($"    public {GetTypeName(fieldType)} {fieldName};");
        }
        sb.AppendLine("}");

        // 목록형 데이터를 위한 래퍼(Wrapper) 클래스 생성
        if (isCollection)
        {
            string listFieldName = $"{className.ToLower()}";
            sb.AppendLine($"\n[Serializable]\npublic class {className}Table\n{{");
            sb.AppendLine($"    public List<{className}> {listFieldName};");
            sb.AppendLine("}");
        }

        File.WriteAllText(Path.Combine(outPathCs, $"{className}.cs"), sb.ToString(), Encoding.UTF8);

        // 스키마 파일 저장(이전 자동화 기능 유산)
        // 현재는 사용되지 않지만 데이터 구조를 확인하는 용도로 남겨둘 수 있음
        // 헤더 목록을 List<string>으로 만든 뒤 간단한 JSON 형태로 저장
        List<string> headers = new List<string>();
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string fieldName = headerRow[i]?.ToString();
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                headers.Add(fieldName);
            }
        }
        string schemaJson = JsonUtility.ToJson(new HeaderSchema { headers = headers });
        Directory.CreateDirectory(outPathSchema);
        File.WriteAllText(Path.Combine(outPathSchema, $"{className}.schema.json"), schemaJson, Encoding.UTF8);
    }

    public static void ConvertSheetToJson(DataTable table, string className, string outJsonPath)
    {
        // 문자열 이름(className)에 해당하는 실제 C# 클래스 Type(설계도)을 찾아옴
        Type dataType = FindTypeInAssemblies(className);
        if (dataType == null)
        {
            Debug.LogError($"Class '{className}' not found. Please ensure it has been generated and compiled.");
            return;
        }

        DataRow headerRow = table.Rows[0];
        // typeof(IData).IsAssignableFrom(dataType) : dataType이라는 클래스가 IData 인터페이스 규칙을 따르는지 확인
        bool isCollection = typeof(IData).IsAssignableFrom(dataType);
        string json;

        // 목록형(Collection) 데이터 처리
        if (isCollection)
        {
            // 리플렉션 객체 생성
            // 래퍼 클래스(MonsterDataTable)의 설계도를 찾아옴
            Type tableType = FindTypeInAssemblies($"{className}Table");
            if (tableType == null)
            { 
                Debug.LogError($"Wrapper class '{className}Table' not found.");
                return;
            }

            // Activator.CreateInstance: 설계도(Type)만 가지고 비어있는 객체를 동적으로 생성
            // object tableInstance = new MonsterDataTable();과 동일한 효과
            object tableInstance = Activator.CreateInstance(tableType);
            object dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));

            // 리플렉션 객체 연결
            string listFieldName = $"{className.ToLower()}"; // 예: "monsterdata"
            // 래퍼 클래스 설계도에서 listFieldName("monsterdata") 이라는 이름의 변수 정보(FieldInfo)를 찾아옴
            FieldInfo listField = tableType.GetField(listFieldName);
            if (listField == null) 
            { 
                Debug.LogError($"Field '{listFieldName}' not found in '{tableType.Name}'.");
                return;
            }

            // listField.SetValue: tableInstance객체의 listField 변수에 dataList 객체 할당
            // tableInstance.monsterdatas = dataList; 와 동일한 효과
            listField.SetValue(tableInstance, dataList);

            // dataList의 설계도에서 "Add"라는 이름의 메서드 정보(MethodInfo)를 찾아옴
            MethodInfo listAddMethod = dataList.GetType().GetMethod("Add");

            // 리플렉션 데이터 채우기
            for (int i = 1; i < table.Rows.Count; i++)
            {
                DataRow dataRow = table.Rows[i];
                if (dataRow[0] == null || string.IsNullOrWhiteSpace(dataRow[0].ToString()))
                {
                    continue;
                }

                // 각 데이터 행마다 비어있는 MonsterData 객체 하나씩 생성
                object dataInstance = Activator.CreateInstance(dataType);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    string fieldName = headerRow[j].ToString();
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        continue;
                    }

                    // MonsterData 클래스 설계도에서 현재 헤더 이름과 일치하는 변수 정보 찾음
                    FieldInfo field = dataType.GetField(fieldName);
                    if (field != null)
                    {
                        // DataTable의 값을 변수의 타입에 맞게 변환
                        object value = SafeChangeType(dataRow[j], field.FieldType);
                        // dataInstance 객체의 field 변수에 변환된 value 할당
                        // dataInstance.hp = 100; 과 동일한 효과
                        field.SetValue(dataInstance, value);
                    }
                }
                // listAddMethod.Invoke: dataList 객체의 Add메서드를 실행하라는 명령
                // dataList.Add(dataInstance); 와 동일한 효과
                listAddMethod.Invoke(dataList, new[] { dataInstance });
            }
            // 모든 데이터가 채워진 래퍼 객체를 JSON으로 변환
            json = JsonUtility.ToJson(tableInstance, true);
        }
        else // 단일 객체 데이터
        {
            // 리플렉션을 이용해 객체를 만들고 변수에 값을 채워넣음
            if (table.Rows.Count < 2)
            {
                Debug.LogWarning($"Single data sheet '{className}' has no data row. Skipping JSON conversion.");
                return;
            }
            DataRow dataRow = table.Rows[1];
            object dataInstance = Activator.CreateInstance(dataType);
            for (int j = 0; j < table.Columns.Count; j++)
            {
                string fieldName = headerRow[j].ToString();
                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                FieldInfo field = dataType.GetField(fieldName);
                if (field != null)
                {
                    object value = SafeChangeType(dataRow[j], field.FieldType);
                    field.SetValue(dataInstance, value);
                }
            }
            json = JsonUtility.ToJson(dataInstance, true);
        }
        File.WriteAllText(Path.Combine(outJsonPath, $"{className}.json"), json, Encoding.UTF8);
    }

    public static Type InferTypeOfValue(object cellValue)
    {
        // 셀 값이 null(아예 비어있음)이면 기본적으로 문자열(string) 타입으로 취급
        if (cellValue == null)
        {
            return typeof(string);
        }

        // 셀 값을 문자열로 변환하고 혹시 모를 앞뒤 공백 제거
        string stringValue = cellValue.ToString().Trim();
        if (string.IsNullOrEmpty(stringValue))
        {
            return typeof(string);
        }

        // 실수 확인
        if (stringValue.EndsWith("f", StringComparison.OrdinalIgnoreCase) || stringValue.Contains("."))
        {
            if (float.TryParse(stringValue.Replace("f", ""), out _))
            {
                return typeof(float);
            }
        }

        // 정수 확인
        if (int.TryParse(stringValue, out _))
        {
            return typeof(int);
        }

        // 불리언 확인
        // TRUE 또는 FALSE(대소문자 무관)라고 입력한 경우 감지
        if (bool.TryParse(stringValue, out _))
        {
            return typeof(bool);
        }

        return typeof(string);
    }

    public static string GetTypeName(Type type)
    {
        // C#의 Type 객체와 코드에서 사용하는 키워드를 1:1로 대응
        if (type == typeof(int))
        {
            return "int";
        }
        if (type == typeof(float))
        {
            return "float";
        }
        if (type == typeof(bool))
        {
            return "bool";
        }

        return "string";
    }

    public static Type FindTypeInAssemblies(string typeName)
    {
        // AppDomain.CurrentDomain.GetAssemblies()는 현재 실행 중인 프로그램에 로드된
        // 모든 코드 라이브러리(어셈블리) 목록을 가져온다. Unity 엔진 코드, 플러그인,
        // 그리고 방금 생성한 클래스가 컴파일된 코드까지 모두 포함
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // 각 라이브러리(assembly) 안에서 typeName과 이름이 같은 Type이 있는지 검색
            var type = assembly.GetType(typeName);
            // Type 찾았다면 즉시 검색을 중단하고 찾은 Type 반환
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    public static object SafeChangeType(object value, Type conversionType)
    {
        // 셀 값이 비어있는 경우(null) 또는 DBNull(데이터베이스에서 온 빈 값)인 경우 처리
        if (value == null || value is DBNull || string.IsNullOrWhiteSpace(value.ToString()))
        {
            // 변환하려는 타입이 값 타입(int, float 등)이면 기본값(0, 0.0f),
            // 참조 타입(string 등)이면 null 반환
            return conversionType.IsValueType ? Activator.CreateInstance(conversionType) : null;
        }

        string valueStr = value.ToString().Trim();
        try
        {
            // float 타입이면서 f로 끝나는 경우 f 제거하고 변환
            if (conversionType == typeof(float) && valueStr.EndsWith("f", StringComparison.OrdinalIgnoreCase))
            {
                return float.Parse(valueStr.Substring(0, valueStr.Length - 1));
            }

            // C#의 기본 타입 변환 기능을 사용하여 값을 목표 타입으로 변환
            // 실패하면 바로 catch 블록으로 점프
            return Convert.ChangeType(valueStr, conversionType);
        }
        catch (FormatException) // FormatException 타입 변환에 실패했을 때 실행
        {
            Debug.LogWarning($"Could not convert '{valueStr}' to type '{conversionType.Name}'. Using default value instead.");

            // 툴 멈추는 대신 기본값 반환하여 작업 계속 진행
            return conversionType.IsValueType ? Activator.CreateInstance(conversionType) : null;
        }
    }
    #endregion
}
