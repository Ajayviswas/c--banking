using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace c__learning
{
    class Program
    {
        static List<User> users = new List<User>();
        static User loggedInUser = null;

        static void Main(string[] args)
        {
            while (true)
            {
                if (loggedInUser == null)
                {
                    Console.WriteLine("\n<--Welcome to Console Banking-->");
                    Console.WriteLine("1.Register");
                    Console.WriteLine("2.Login");
                    Console.WriteLine("3.Exit");
                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": Register(); break;
                        case "2": Login(); break;
                        case "3": return;
                        default: Console.WriteLine("Invalid option."); break;
                    }
                }
                else
                {
                    Console.WriteLine($"\n--- Welcome, {loggedInUser.Username}! ---");
                    Console.WriteLine("1. Open Account");
                    Console.WriteLine("2. Deposit");
                    Console.WriteLine("3. Withdraw");
                    Console.WriteLine("4. View Statement");
                    Console.WriteLine("5. Calculate Interest");
                    Console.WriteLine("6. Check Balance");
                    Console.WriteLine("7. Logout");
                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": OpenAccount(); break;
                        case "2": Deposit(); break;
                        case "3": Withdraw(); break;
                        case "4": ViewStatement(); break;
                        case "5": CalculateInterest(); break;
                        case "6": CheckBalance(); break;
                        case "7": loggedInUser = null; break;
                        default: Console.WriteLine("Invalid option."); break;
                    }
                }
            }
        }

        static void Register()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            users.Add(new User(username, password));
            Console.WriteLine("Registration successful!");
        }

        static void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            loggedInUser = users.Find(user => user.Username == username && user.Password == password);
            if (loggedInUser == null)
                Console.WriteLine("Invalid credentials.");
            else
                Console.WriteLine("Login successful!");
        }

        static void OpenAccount()
        {
            Console.Write("Enter account holder's name: ");
            string holderName = Console.ReadLine();
            Console.Write("Enter account type (savings/checking): ");
            string accountType = Console.ReadLine();
            Console.Write("Enter initial deposit: ");
            decimal initialDeposit = decimal.Parse(Console.ReadLine());

            Account newAccount = new Account(holderName, accountType, initialDeposit);
            loggedInUser.Accounts.Add(newAccount);
            Console.WriteLine($"Account opened successfully! Account Number: {newAccount.AccountNumber}");
        }

        static void Deposit()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.Write("Enter deposit amount: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                account.Deposit(amount);
                Console.WriteLine("Deposit successful.");
            }
        }

        static void Withdraw()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.Write("Enter withdrawal amount: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                if (account.Withdraw(amount))
                    Console.WriteLine("Withdrawal successful.");
                else
                    Console.WriteLine("Insufficient funds.");
            }
        }

        static void ViewStatement()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.WriteLine("\n--- Transaction Statement ---");
                foreach (var transaction in account.Transactions)
                {
                    Console.WriteLine($"{transaction.Date} - {transaction.Type} - ${transaction.Amount}");
                }
            }
        }

        static void CalculateInterest()
        {
            Account account = SelectAccount();
            if (account != null && account.AccountType == "savings")
            {
                decimal interestRate = 0.02m; // Fixed interest rate for simplicity
                account.ApplyInterest(interestRate);
                Console.WriteLine("Monthly interest added.");
            }
            else
            {
                Console.WriteLine("Interest calculation is only applicable to savings accounts.");
            }
        }

        static void CheckBalance()
        {
            Account account = SelectAccount();
            if (account != null)
                Console.WriteLine($"Current Balance: ${account.Balance}");
        }

        static Account SelectAccount()
        {
            if (loggedInUser.Accounts.Count == 0)
            {
                Console.WriteLine("No accounts found. Please open an account first.");
                return null;
            }

            Console.Write("Enter account number: ");
            string accountNumber = Console.ReadLine();
            Account account = loggedInUser.Accounts.Find(acc => acc.AccountNumber == accountNumber);
            if (account == null)
                Console.WriteLine("Account not found.");

            return account;
        }
    }

    class User
    {
        public string Username { get; }
        public string Password { get; }
        public List<Account> Accounts { get; } = new List<Account>();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    class Account
    {
        private static int accountCounter = 1000;
        public string AccountNumber { get; }
        public string AccountHolderName { get; }
        public string AccountType { get; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; } = new List<Transaction>();
        private DateTime lastInterestDate = DateTime.MinValue;

        public Account(string holderName, string accountType, decimal initialDeposit)
        {
            AccountNumber = (accountCounter++).ToString();
            AccountHolderName = holderName;
            AccountType = accountType.ToLower();
            Balance = initialDeposit;
            Transactions.Add(new Transaction("Initial Deposit", initialDeposit));
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Transactions.Add(new Transaction("Deposit", amount));
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Transactions.Add(new Transaction("Withdrawal", amount));
                return true;
            }
            return false;
        }

        public void ApplyInterest(decimal interestRate)
        {
            DateTime currentDate = DateTime.Now;
            if ((currentDate - lastInterestDate).Days >= 30)
            {
                decimal interest = Balance * interestRate;
                Deposit(interest);
                lastInterestDate = currentDate;
            }
        }
    }

    class Transaction
    {
        public string TransactionId { get; }
        public DateTime Date { get; }
        public string Type { get; }
        public decimal Amount { get; }

        public Transaction(string type, decimal amount)
        {
            TransactionId = Guid.NewGuid().ToString();
            Date = DateTime.Now;
            Type = type;
            Amount = amount;
        }
    }

}
