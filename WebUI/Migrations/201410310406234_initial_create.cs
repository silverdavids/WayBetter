namespace WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 50),
                        DateE = c.DateTime(),
                        AmountE = c.Double(),
                        Category = c.String(maxLength: 50),
                        AdminE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Receipts",
                c => new
                    {
                        ReceiptId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 50),
                        ReceiptDate = c.DateTime(),
                        Stake = c.Double(),
                        TotalOdds = c.Double(),
                        ReceiptStatus = c.Int(nullable: false),
                        IsCanceled = c.Boolean(nullable: false),
                        SetNo = c.Int(nullable: false),
                        SetSize = c.Int(nullable: false),
                        SubmitedSize = c.Int(nullable: false),
                        WonSize = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Serial = c.Guid(),
                        ReceiptStatus_StatusId = c.Int(),
                    })
                .PrimaryKey(t => t.ReceiptId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.UserId)
                .ForeignKey("dbo.ReceiptStatus", t => t.ReceiptStatus_StatusId)
                .Index(t => t.UserId)
                .Index(t => t.BranchId)
                .Index(t => t.ReceiptStatus_StatusId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        BranchId = c.Int(nullable: false, identity: true),
                        BranchName = c.String(),
                        BranchCode = c.String(),
                        Location = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        MaxCash = c.Decimal(nullable: false, storeType: "money"),
                        MinStake = c.Decimal(storeType: "money"),
                        ManagerId = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchTypeId = c.Int(),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.BranchId)
                .ForeignKey("dbo.BranchTypes", t => t.BranchTypeId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.BranchTypeId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.BranchTypes",
                c => new
                    {
                        BranchTypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.BranchTypeId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false),
                        CompanyLocation = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        HireDate = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        MiddleName = c.String(),
                        Surname = c.String(nullable: false, maxLength: 50),
                        Gender = c.Int(nullable: false),
                        Email = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PhoneNo = c.String(),
                        Mobile = c.String(),
                        Photo = c.String(),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        ShiftId = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        AssignedBy = c.Int(nullable: false),
                        IsClosed = c.Boolean(),
                        StartCash = c.Decimal(nullable: false, storeType: "money"),
                        CashIn = c.Decimal(nullable: false, storeType: "money"),
                        CashOut = c.Decimal(nullable: false, storeType: "money"),
                        PersonId = c.Int(nullable: false),
                        TerminalId = c.Int(),
                    })
                .PrimaryKey(t => t.ShiftId)
                .ForeignKey("dbo.Employees", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.PersonId)
                .Index(t => t.TerminalId);
            
            CreateTable(
                "dbo.Terminals",
                c => new
                    {
                        TerminalId = c.Int(nullable: false, identity: true),
                        TerminalName = c.String(),
                        IpAddress = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        isActive = c.Boolean(nullable: false),
                        BranchId = c.Int(),
                    })
                .PrimaryKey(t => t.TerminalId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.Bets",
                c => new
                    {
                        BetId = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        RecieptId = c.Int(nullable: false),
                        GameBetStatus = c.Int(nullable: false),
                        BetOptionId = c.Int(nullable: false),
                        BetOdd = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BetId)
                .ForeignKey("dbo.BetOptions", t => t.BetOptionId, cascadeDelete: true)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Receipts", t => t.RecieptId, cascadeDelete: true)
                .Index(t => t.MatchId)
                .Index(t => t.RecieptId)
                .Index(t => t.BetOptionId);
            
            CreateTable(
                "dbo.BetOptions",
                c => new
                    {
                        BetOptionId = c.Int(nullable: false, identity: true),
                        Option = c.String(nullable: false),
                        Line = c.String(),
                        BetCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BetOptionId)
                .ForeignKey("dbo.BetCategories", t => t.BetCategoryId, cascadeDelete: true)
                .Index(t => t.BetCategoryId);
            
            CreateTable(
                "dbo.BetCategories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MinimumSize = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        BetServiceMatchNo = c.Int(nullable: false),
                        League = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        GameStatus = c.String(),
                        AwayTeamId = c.Int(nullable: false),
                        HomeTeamId = c.Int(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        HomeScore = c.Int(nullable: false),
                        AwayScore = c.Int(nullable: false),
                        HalfTimeHomeScore = c.Int(nullable: false),
                        HalfTimeAwayScore = c.Int(nullable: false),
                        ResultStatus = c.Int(),
                    })
                .PrimaryKey(t => t.BetServiceMatchNo)
                .ForeignKey("dbo.Teams", t => t.AwayTeamId, cascadeDelete: false)
                .ForeignKey("dbo.Teams", t => t.HomeTeamId, cascadeDelete: false)
                .Index(t => t.AwayTeamId)
                .Index(t => t.HomeTeamId);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamId = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        LeagueId = c.Int(),
                        DateRegistered = c.DateTime(),
                        CountryId = c.Int(nullable: false),
                        IsNationalTeam = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TeamId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.Leagues", t => t.LeagueId)
                .Index(t => t.LeagueId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Leagues",
                c => new
                    {
                        LeagueId = c.Int(nullable: false, identity: true),
                        LeagueName = c.String(),
                        CountryId = c.Int(),
                        Rating = c.Int(),
                    })
                .PrimaryKey(t => t.LeagueId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        StartYear = c.Int(nullable: false),
                        EndYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StartYear, t.EndYear });
            
            CreateTable(
                "dbo.MatchOdds",
                c => new
                    {
                        BetServiceMatchNo = c.Int(nullable: false),
                        BetOptionId = c.Int(nullable: false),
                        MatchOddId = c.Int(nullable: false, identity: true),
                        Odd = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.MatchOddId)
                .ForeignKey("dbo.BetOptions", t => t.BetOptionId, cascadeDelete: true)
                .ForeignKey("dbo.Matches", t => t.BetServiceMatchNo, cascadeDelete: true)
                .Index(t => t.BetServiceMatchNo)
                .Index(t => t.BetOptionId);
            
            CreateTable(
                "dbo.ShortMatchCodes",
                c => new
                    {
                        MatchNo = c.Int(nullable: false),
                        SetNo = c.Int(nullable: false),
                        ShortCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MatchNo)
                .ForeignKey("dbo.Matches", t => t.MatchNo)
                .Index(t => t.MatchNo);
            
            CreateTable(
                "dbo.CancelReciepts",
                c => new
                    {
                        RecieptId = c.Int(nullable: false),
                        RecievedFrom = c.String(maxLength: 50),
                        Reason = c.String(),
                        CancelationTime = c.DateTime(),
                        ApprovalStatus = c.String(maxLength: 50),
                        Approvaltime = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RecieptId)
                .ForeignKey("dbo.Receipts", t => t.RecieptId)
                .Index(t => t.RecieptId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 50),
                        ContentType = c.String(nullable: false, maxLength: 50),
                        Data = c.Binary(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LiveMatches",
                c => new
                    {
                        LiveMatchNo = c.String(nullable: false, maxLength: 128),
                        BetServiceMatchNo = c.Int(nullable: false),
                        SetDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LiveMatchNo)
                .ForeignKey("dbo.Matches", t => t.BetServiceMatchNo, cascadeDelete: true)
                .Index(t => t.BetServiceMatchNo);
            
            CreateTable(
                "dbo.ReceiptStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false),
                        StatusName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        ResultId = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        OptionId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResultId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Statements",
                c => new
                    {
                        StatementId = c.Int(nullable: false, identity: true),
                        Account = c.String(maxLength: 50),
                        Transcation = c.String(),
                        Method = c.String(maxLength: 50),
                        Controller = c.String(maxLength: 50),
                        StatetmentDate = c.DateTime(),
                        Serial = c.String(maxLength: 255),
                        BalBefore = c.Double(),
                        Amount = c.Double(),
                        BalAfter = c.Double(),
                        Error = c.Boolean(nullable: false),
                        Comment = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.StatementId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Firstname = c.String(),
                        Surname = c.String(),
                        DateOfBirth = c.DateTime(),
                        Gender = c.String(),
                        Address = c.String(),
                        MobileContact = c.String(),
                        IsActivated = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SeasonLeagues",
                c => new
                    {
                        Season_StartYear = c.Int(nullable: false),
                        Season_EndYear = c.Int(nullable: false),
                        League_LeagueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Season_StartYear, t.Season_EndYear, t.League_LeagueId })
                .ForeignKey("dbo.Seasons", t => new { t.Season_StartYear, t.Season_EndYear }, cascadeDelete: true)
                .ForeignKey("dbo.Leagues", t => t.League_LeagueId, cascadeDelete: true)
                .Index(t => new { t.Season_StartYear, t.Season_EndYear })
                .Index(t => t.League_LeagueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Receipts", "ReceiptStatus_StatusId", "dbo.ReceiptStatus");
            DropForeignKey("dbo.LiveMatches", "BetServiceMatchNo", "dbo.Matches");
            DropForeignKey("dbo.CancelReciepts", "RecieptId", "dbo.Receipts");
            DropForeignKey("dbo.Receipts", "UserId", "dbo.Accounts");
            DropForeignKey("dbo.Bets", "RecieptId", "dbo.Receipts");
            DropForeignKey("dbo.Bets", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.ShortMatchCodes", "MatchNo", "dbo.Matches");
            DropForeignKey("dbo.MatchOdds", "BetServiceMatchNo", "dbo.Matches");
            DropForeignKey("dbo.MatchOdds", "BetOptionId", "dbo.BetOptions");
            DropForeignKey("dbo.Teams", "LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.SeasonLeagues", "League_LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.SeasonLeagues", new[] { "Season_StartYear", "Season_EndYear" }, "dbo.Seasons");
            DropForeignKey("dbo.Leagues", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Matches", "HomeTeamId", "dbo.Teams");
            DropForeignKey("dbo.Teams", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Matches", "AwayTeamId", "dbo.Teams");
            DropForeignKey("dbo.Bets", "BetOptionId", "dbo.BetOptions");
            DropForeignKey("dbo.BetOptions", "BetCategoryId", "dbo.BetCategories");
            DropForeignKey("dbo.Receipts", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Shifts", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Shifts", "PersonId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branches", "BranchTypeId", "dbo.BranchTypes");
            DropIndex("dbo.SeasonLeagues", new[] { "League_LeagueId" });
            DropIndex("dbo.SeasonLeagues", new[] { "Season_StartYear", "Season_EndYear" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LiveMatches", new[] { "BetServiceMatchNo" });
            DropIndex("dbo.CancelReciepts", new[] { "RecieptId" });
            DropIndex("dbo.ShortMatchCodes", new[] { "MatchNo" });
            DropIndex("dbo.MatchOdds", new[] { "BetOptionId" });
            DropIndex("dbo.MatchOdds", new[] { "BetServiceMatchNo" });
            DropIndex("dbo.Leagues", new[] { "CountryId" });
            DropIndex("dbo.Teams", new[] { "CountryId" });
            DropIndex("dbo.Teams", new[] { "LeagueId" });
            DropIndex("dbo.Matches", new[] { "HomeTeamId" });
            DropIndex("dbo.Matches", new[] { "AwayTeamId" });
            DropIndex("dbo.BetOptions", new[] { "BetCategoryId" });
            DropIndex("dbo.Bets", new[] { "BetOptionId" });
            DropIndex("dbo.Bets", new[] { "RecieptId" });
            DropIndex("dbo.Bets", new[] { "MatchId" });
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.Shifts", new[] { "TerminalId" });
            DropIndex("dbo.Shifts", new[] { "PersonId" });
            DropIndex("dbo.Employees", new[] { "BranchId" });
            DropIndex("dbo.Branches", new[] { "CompanyId" });
            DropIndex("dbo.Branches", new[] { "BranchTypeId" });
            DropIndex("dbo.Receipts", new[] { "ReceiptStatus_StatusId" });
            DropIndex("dbo.Receipts", new[] { "BranchId" });
            DropIndex("dbo.Receipts", new[] { "UserId" });
            DropTable("dbo.SeasonLeagues");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Statements");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Results");
            DropTable("dbo.ReceiptStatus");
            DropTable("dbo.LiveMatches");
            DropTable("dbo.Documents");
            DropTable("dbo.CancelReciepts");
            DropTable("dbo.ShortMatchCodes");
            DropTable("dbo.MatchOdds");
            DropTable("dbo.Seasons");
            DropTable("dbo.Leagues");
            DropTable("dbo.Countries");
            DropTable("dbo.Teams");
            DropTable("dbo.Matches");
            DropTable("dbo.BetCategories");
            DropTable("dbo.BetOptions");
            DropTable("dbo.Bets");
            DropTable("dbo.Terminals");
            DropTable("dbo.Shifts");
            DropTable("dbo.Employees");
            DropTable("dbo.Companies");
            DropTable("dbo.BranchTypes");
            DropTable("dbo.Branches");
            DropTable("dbo.Receipts");
            DropTable("dbo.Accounts");
        }
    }
}
