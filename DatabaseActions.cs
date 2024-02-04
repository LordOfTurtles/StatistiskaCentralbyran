using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
namespace SCB;

static class SCBDATABASE
{
    static string connectionString = "Server=localhost; User ID=root; Database=SCB";
    static IDbConnection connection = new MySqlConnection(connectionString);

    public static List<dynamic> GetAllQuestionnaires()
    {
        string sql = "SELECT Id, Title " +
        "FROM Questionnaires";
        return connection.Query<dynamic>(sql).ToList();
    }

    public static List<Participant> GetAllParticipants()
    {
        string sql = "SELECT p.Id, p.FirstName, p.LastName, p.PersonNr, p.PhoneNr, p.Email, p.Password " +
        "FROM Participants p ";
        return connection.Query<Participant>(sql).ToList();
    }

    public static Participant AddParticipant(Participant p)
    {
        var parameters = new { FirstName=p.FirstName, LastName=p.LastName, Email=p.Email, PhoneNr=p.PhoneNr, PersonNr=p.PersonNr, Password=p.Password};
            string sql=$"INSERT INTO Participants(FirstName, LastName, Email, PhoneNr, PersonNr, Password) VALUES(@FirstName, @LastName, @Email, @PhoneNr, @PersonNr, @Password);";
            System.Console.WriteLine(sql);
            connection.Execute(sql, parameters);
            System.Console.WriteLine(sql);
            sql = "SELECT p.Id, p.FirstName, p.LastName, p.PersonNr, p.PhoneNr, p.Email, p.Password " +
            "FROM Participants p " +
            $"WHERE p.Email='{p.Email}' AND p.PersonNr='{p.PersonNr}'";
            return connection.QuerySingle<Participant>(sql);
    }

    public static List<dynamic> GetQuestions(int i)
    {
        string sql = "SELECT q.Id, q.Body, qt.Name, aq.IsMandatory " +
        "FROM Questions q " +
        "INNER JOIN AskedQuestions aq ON aq.QuestionId=q.Id " +
        "INNER JOIN Questionnaires qu ON aq.QuestionnaireId=qu.Id " +
        "INNER JOIN QuestionTypes qt ON q.QuestionTypeId=qt.Id " +
        $"WHERE qu.Id='{i}'";
        Console.WriteLine(sql);
        return connection.Query<dynamic>(sql).ToList();
    }

    public static List<dynamic> GetMultipleChoice(int i)
    {
        string sql = "SELECT m.Description, c.Symbol, c.Id " +
        "FROM MultipleChoiceOptions m " +
        "INNER JOIN MultipleChoiceSymbols c ON m.ChoiceSymbolId=c.Id " +
        $"WHERE m.QuestionId='{i}'";
        return connection.Query<dynamic>(sql).ToList();
    }

    public static void AddResponse(int quid, int qid, string type, string a)
    {
        var parameters = new {QuestionId=qid, QuestionnaireId=quid, ParticipantId=ConsoleUI.CurrentParticipant!.Id, ChoiceVal=a};
        string sql = "INSERT INTO Responses (QuestionId, QuestionnaireId, ParticipantId";
        switch(type)
        {
            case "MultipleChoice":
                sql += ", ChoiceSymbolId) ";
            break;
            case "Scale":
                sql += ", ScaleVal) ";
            break;
            case "FreeText":
                sql += ", FreetextVal) ";
            break;
        }
        sql += "VALUES (@QuestionId, @QuestionnaireId, @ParticipantId, @ChoiceVal);";
        System.Console.WriteLine(sql);
        connection.Execute(sql, parameters);
    }

}