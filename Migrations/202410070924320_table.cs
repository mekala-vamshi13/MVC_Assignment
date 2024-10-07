namespace User_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordDetails",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.UserDetails", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UserDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DOB = c.DateTime(nullable: false),
                        Gender = c.String(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PasswordDetails", "UserID", "dbo.UserDetails");
            DropIndex("dbo.PasswordDetails", new[] { "UserID" });
            DropTable("dbo.UserDetails");
            DropTable("dbo.PasswordDetails");
        }
    }
}
