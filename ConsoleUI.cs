
using System.Net;
using System.Runtime.CompilerServices;

namespace SCB;
static class ConsoleUI
{
    public static Participant? CurrentParticipant = null;

    public static Administrator? CurrentAdmin = null;
    public static void MainMenu()
    {
        bool isRunning = true;
        while(isRunning)
        {
            Console.WriteLine("Välkommen till Statistiska Centralbyrån");
            Console.WriteLine("Vänligen välj ett alternativ: \n1.Deltagare \n2.Administratör");
            string userInput = Console.ReadLine()!;
            if(userInput == "1")
            {
                ParticipantMenu();
            }
            else if(userInput == "2")
            {
                AdminMenu();
            }
            else
            {
                PrintErrorMessenger(null);
            }
        }      
    }
    public static void ParticipantMenu()
    {
        while(CurrentParticipant == null)
        {
            Console.Clear();
            Console.WriteLine("Vänligen välj ett alternativ: \n1.Logga in \n2.Skapa ny användare \n3.Gå tillbaka");
            string userInput = Console.ReadLine()!;
            if(userInput == "1")
                {
                    CurrentParticipant = ParticipantLogIn();
                }
                else if(userInput == "2")
                {
                    CurrentParticipant = RegisterNewParticipant();
                }
                else if(userInput == "3")
                {
                    MainMenu();
                }
                else
                {
                    PrintErrorMessenger(null);
                }
        }
        while(CurrentParticipant != null)
        {
            Console.Clear();
            Console.WriteLine($"Inloggad som {CurrentParticipant.FirstName} {CurrentParticipant.LastName}");
            Console.WriteLine("Vänligen välj ett alternativ: \n1.Svara på enkät \n2.Se tidigare svar \n3.Logga ut");
            string userInput2 = Console.ReadLine()!;
            if(userInput2 == "1")
                {
                    BrowseQuestionnaires();
                }
                else if(userInput2 == "2")
                {
                    AdminMenu();
                }
                else if(userInput2 == "3")
                {
                    CurrentParticipant = null;
                }
                else
                {
                    PrintErrorMessenger(null);
                }
        }
    }
    public static void AdminMenu()
    {
        while(CurrentAdmin == null)
        {
            Console.Clear();
            Console.WriteLine("Vänligen välj ett alternativ: \n1.Logga in \n2.Skapa ny användare \n3.Gå tillbaka");
            string userInput = Console.ReadLine()!;
            if(userInput == "1")
                {
                    CurrentParticipant = ParticipantLogIn();
                }
                else if(userInput == "2")
                {
                    CurrentParticipant = RegisterNewParticipant();
                }
                else if(userInput == "3")
                {
                    MainMenu();
                }
                else
                {
                    PrintErrorMessenger(null);
                }
        }
    }
    public static void PrintErrorMessenger(string? message)
    {
        if(message == null)
        {
            Console.WriteLine("Felmeddelande: felaktig inmatning");
        }
        else
        {
            Console.WriteLine($"Felmeddelande: {message}");
        }
        Console.Write("Tryck på valfri tangent för att fortsätta");
        Console.ReadKey();
    }
    public static Participant ParticipantLogIn()
    {
        List<Participant> participants = SCBDATABASE.GetAllParticipants();
        bool isRunning = true;
        while(isRunning)
        {
            Console.Write("Vänligen skriv in din emailadress: ");
            string email = Console.ReadLine()!;
            if(participants.Exists(x => x.Email!.Contains(email)))
            {
                bool isRunning2 = true;
                Participant? p = participants.Find(x => x.Email!.Contains(email));
                while(isRunning2)
                {
                    Console.Write($"Vänligen skriv lösenord för {p!.Email}: ");
                    string password = Console.ReadLine()!;
                    if(password == p.Password)
                    {
                        return p;
                    }
                    else
                    {
                        Console.WriteLine("Felaktigt lösenord, försök igen? [j]a/[n]ej");
                        string userInput = Console.ReadLine()!;
                        if(userInput.ToLower() == "n")
                        {
                            isRunning2 = false;
                        }
                    }
                }     
            }
            else
            {
                Console.WriteLine("Mailadress existerar ej, försök igen? [j]a/[n]ej");
                string userInput = Console.ReadLine()!;
                if(userInput.ToLower() == "n")
                {
                    isRunning = false;
                }
            }
        }
    return null!;
    }

    public static Participant RegisterNewParticipant()
    {
        Participant p = new Participant();
        Console.Write("Vänligen skriv ditt förnamn: ");
        p.FirstName = Console.ReadLine()!;
        Console.Write("Vänligen skriv ditt efternamn: ");
        p.LastName = Console.ReadLine()!;
        Console.Write("Vänligen skriv ditt personnummer: ");
        p.PersonNr = Console.ReadLine()!;
        Console.Write("Vänligen skriv ditt telefonnummer: ");
        p.PhoneNr = Console.ReadLine()!;
        Console.Write("Vänligen skriv din mailadress: ");
        p.Email = Console.ReadLine()!;
        Console.Write("Vänligen skriv ett lösenord: ");
        p.Password = Console.ReadLine()!;
        Console.WriteLine($"Stämmer dessa uppgifter? [j/n] \nNamn: {p.FirstName} {p.LastName} \nPersonnummer: {p.PersonNr} \nTelefonnummer: {p.PhoneNr} \nEmail: {p.Email}");
        string userInput = Console.ReadLine()!;
        if(userInput == "j")
        {
            return SCBDATABASE.AddParticipant(p);
        }
        else return null!;
    }
    public static void BrowseQuestionnaires()
    {
        List<dynamic> questionnaires = SCBDATABASE.GetAllQuestionnaires();
        int i = 1;
        foreach(dynamic d in questionnaires)
        {
            Console.WriteLine($"{i}. {d.Title}");
            i++;
        }
        Console.WriteLine("Vänligen välj önskad enkäts nummer:");
        int userInput = int.Parse(Console.ReadLine()!);
        int b = questionnaires[userInput-1].Id;
        AnswerQuestionnaire(b);
    }
    public static void AnswerQuestionnaire(int i)
    {
        List<dynamic> questions = SCBDATABASE.GetQuestions(i);
        Console.ReadKey();
        foreach(dynamic q in questions)
        {
            Console.WriteLine(q.Body);
            switch(q.Name)
            {
                case "MultipleChoice":
                    List<dynamic> options = SCBDATABASE.GetMultipleChoice(q.Id);
                    foreach(dynamic o in options)
                    {
                        Console.WriteLine($"{o.Symbol}. {o.Description}");
                    }
                    bool makingchoice = true;
                    while(makingchoice)
                    {
                        Console.Write("Välj alternativ:");
                        string userInput = Console.ReadLine()!;
                        if(options.Exists(x => x.Symbol!.Contains(userInput.ToUpper())))
                        {
                            dynamic? a = options.Find(x => x.Symbol!.Contains(userInput));
                            int choice = a!.Id;

                            SCBDATABASE.AddResponse(i, q.Id, q.Name, choice.ToString());
                        }
                    }

                break;
                case "Scale":

                break;
                case "FreeText":

                break;
            }
        }
    }
}
class Participant
{
    public int Id {get; set;}
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public string? PersonNr {get; set;}
    public string? PhoneNr {get; set;}
    public string? Email {get; set;}
    public string? Password {get; set;}
}

class Administrator
{
    public int Id {get; set;}
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public string? Email {get; set;}
    public string? Password {get; set;}
}