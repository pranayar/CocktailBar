## STEPS
1. Import/Clone the code
2. Set up the database on the local/remote server, create a schema named cocktailApp and execute the commands in DatabaseScript.txt file.
3. Make sure the connection string in appsettings.json file (DefaultConnection) is as per your environment!
3. Run the code.

#Flow:
##User side
1. The homescreen has 3 options to take you to the menu of the fruit, beer and vodka cocktails as well as a login button
2. To sign up, click on login and the link on the left side allows you to go to the sign up page
3. After sign in is successful, add to cart option is visible. Optional items are present below in all menus
4. After add to cart you can click on buy now and the items will be moved to the order history section and cart will be cleared.

##Admin side
1. Admin has only 2 users with credentials as follows:
Username=admin1
password=admin@123

Username=admin2
password=admin@123

2. To access the admin console, you need to go via the URL for example: localhost/Admin/Login
3. Admin credentials need to be modified/added directly via DBMS
4. 3 options are present to fetch All customers, products and orders as well as option to add, modify, delete and view (CRUD)


