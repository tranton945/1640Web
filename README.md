# Group4_1640Web
Step 1: After clone the project go to Tool -> NuGet Package Manager -> Package Manager Console
Step 2: In the Package Manager Console window, enter the command "Update-database"
Bước 3: go to View -> SQL Server Object Explorer 
Step 4: In the SQL Server Object Explorer window go to SQL Server -> (localdb)/MSSQLLocalDB -> Database -> RoleBaseWeb
Note: if "RoleBaseWeb" is not found, check the appsetting.json file in the directory tree. Find "ConnectionStrings: and "DefaultConnection", the name of the database is the value Catalog=RoleBaseWeb
Step 5: Go to Table -> right-click and select View data, here create structured data
Id		  Name			    NormalizedName
1		    Admin			    Admin
2 		  Staff			    Staff
3		    Manager		    Manager
4		    Coordinator		Coordinator

Step 6: launch the application -> register an account with the structure
username@admin.gmail.com
username@staff.gmail.com
username@manager.gmail.com
username@coordinator.gmail.com


=============================================================================================
