namespace Memo_Logistics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplexDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Budget = c.Decimal(nullable: false, storeType: "money"),
                        Cargo = c.String(maxLength: 50),
                        OrderDate = c.DateTime(nullable: false),
                        DriverID = c.Int(),
                    })
                .PrimaryKey(t => t.CustomerID)
                .ForeignKey("dbo.Drivers", t => t.DriverID)
                .Index(t => t.DriverID);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        HireDate = c.DateTime(nullable: false),
                        TruckID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OfficeAssignment",
                c => new
                    {
                        DriverID = c.Int(nullable: false),
                        Location = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.DriverID)
                .ForeignKey("dbo.Drivers", t => t.DriverID)
                .Index(t => t.DriverID);

            CreateTable(
                "dbo.Trucks",
                c => new
                {
                    TruckID = c.Int(nullable: false),
                    Truck = c.String(maxLength: 50),
                    Mileage = c.Int(nullable: false),
                    CustomerID = c.Int(),
                })
                .PrimaryKey(t => t.TruckID);
           

            CreateTable(
                "dbo.Logistics",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(),
                    DriverID = c.Int(),
                    DeparturePoint = c.String(nullable: false),
                    ArrivalPoint = c.String(nullable: false),
                    Distance = c.Int(nullable: false),
                    DepartureDate = c.DateTime(nullable: false),
                    ArrivalDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID);
                
            
            CreateTable(
                "dbo.TruckDrivers",
                c => new
                    {
                        TruckID = c.Int(nullable: false),
                        DriverID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TruckID, t.DriverID })
                .ForeignKey("dbo.Trucks", t => t.TruckID, cascadeDelete: true)
                .ForeignKey("dbo.Drivers", t => t.DriverID, cascadeDelete: true)
                .Index(t => t.TruckID)
                .Index(t => t.DriverID);
            
            CreateStoredProcedure(
                "dbo.Customers_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        Cargo = p.String(maxLength: 50),
                        OrderDate = p.DateTime(),
                        DriverID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Customers]([Name], [Budget], [Cargo], [OrderDate], [DriverID])
                      VALUES (@Name, @Budget, @Cargo, @OrderDate, @DriverID)
                      
                      DECLARE @CustomerID int
                      SELECT @CustomerID = [CustomerID]
                      FROM [dbo].[Customers]
                      WHERE @@ROWCOUNT > 0 AND [CustomerID] = scope_identity()
                      
                      SELECT t0.[CustomerID]
                      FROM [dbo].[Customers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[CustomerID] = @CustomerID"
            );
            
            CreateStoredProcedure(
                "dbo.Customers_Update",
                p => new
                    {
                        CustomerID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        Cargo = p.String(maxLength: 50),
                        OrderDate = p.DateTime(),
                        DriverID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [Name] = @Name, [Budget] = @Budget, [Cargo] = @Cargo, [OrderDate] = @OrderDate, [DriverID] = @DriverID
                      WHERE ([CustomerID] = @CustomerID)"
            );
            
            CreateStoredProcedure(
                "dbo.Customers_Delete",
                p => new
                    {
                        CustomerID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Customers]
                      WHERE ([CustomerID] = @CustomerID)"
            );
            
            CreateStoredProcedure(
                "dbo.Logistics_Insert",
                p => new
                    {
                        CustomerID = p.Int(),
                        DriverID = p.Int(),
                        DeparturePoint = p.String(),
                        ArrivalPoint = p.String(),
                        Distance = p.Int(),
                        DepartureDate = p.DateTime(),
                        ArrivalDate = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Logistics]([CustomerID], [DriverID], [DeparturePoint], [ArrivalPoint], [Distance], [DepartureDate], [ArrivalDate])
                      VALUES (@CustomerID, @DriverID, @DeparturePoint, @ArrivalPoint, @Distance, @DepartureDate, @ArrivalDate)
                      
                      DECLARE @ID int
                      SELECT @ID = [ID]
                      FROM [dbo].[Logistics]
                      WHERE @@ROWCOUNT > 0 AND [ID] = scope_identity()
                      
                      SELECT t0.[ID]
                      FROM [dbo].[Logistics] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ID] = @ID"
            );
            
            CreateStoredProcedure(
                "dbo.Logistics_Update",
                p => new
                    {
                        ID = p.Int(),
                        CustomerID = p.Int(),
                        DriverID = p.Int(),
                        DeparturePoint = p.String(),
                        ArrivalPoint = p.String(),
                        Distance = p.Int(),
                        DepartureDate = p.DateTime(),
                        ArrivalDate = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Logistics]
                      SET [CustomerID] = @CustomerID, [DriverID] = @DriverID, [DeparturePoint] = @DeparturePoint, [ArrivalPoint] = @ArrivalPoint, [Distance] = @Distance, [DepartureDate] = @DepartureDate, [ArrivalDate] = @ArrivalDate
                      WHERE ([ID] = @ID)"
            );
            
            CreateStoredProcedure(
                "dbo.Logistics_Delete",
                p => new
                    {
                        ID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Logistics]
                      WHERE ([ID] = @ID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Logistics_Delete");
            DropStoredProcedure("dbo.Logistics_Update");
            DropStoredProcedure("dbo.Logistics_Insert");
            DropStoredProcedure("dbo.Customers_Delete");
            DropStoredProcedure("dbo.Customers_Update");
            DropStoredProcedure("dbo.Customers_Insert");
            DropForeignKey("dbo.Logistics", "DriverID", "dbo.Drivers");
            DropForeignKey("dbo.Logistics", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Customers", "DriverID", "dbo.Drivers");
            DropForeignKey("dbo.TruckDrivers", "DriverID", "dbo.Drivers");
            DropForeignKey("dbo.TruckDrivers", "TruckID", "dbo.Trucks");
            DropForeignKey("dbo.Trucks", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.OfficeAssignment", "DriverID", "dbo.Drivers");
            DropIndex("dbo.TruckDrivers", new[] { "DriverID" });
            DropIndex("dbo.TruckDrivers", new[] { "TruckID" });
            DropIndex("dbo.Logistics", new[] { "DriverID" });
            DropIndex("dbo.Logistics", new[] { "CustomerID" });
            DropIndex("dbo.Trucks", new[] { "CustomerID" });
            DropIndex("dbo.OfficeAssignment", new[] { "DriverID" });
            DropIndex("dbo.Customers", new[] { "DriverID" });
            DropTable("dbo.TruckDrivers");
            DropTable("dbo.Logistics");
            DropTable("dbo.Trucks");
            DropTable("dbo.OfficeAssignment");
            DropTable("dbo.Drivers");
            DropTable("dbo.Customers");
        }
    }
}
