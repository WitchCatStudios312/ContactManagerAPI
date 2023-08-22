I wrote a simple API for a Contact Manager using .NET Core.
I used EntityFramework to store the data in an in memory database because this would simplify the process of testing and deploying this simple app.
Test data is populated on Startup.
Since there are currently no business requirements specifying certain formats for postal codes or phone numbers, I decided to leave the formats of these fields open since they can be different by country etc.
I did not want to over validate the data. I saw this app being used the same way as if the user had a paper address book. They can put in the fields whatever data they wish.
I did provide some validation for the email address field, but I left the rest open.
I did not require any contact fields for the same reason, I wanted the user to have more flexibility to store whatever data they had available for each contact.
For the profile image I have 2 fields - one a string and one a byte array.
This way the client app can send the images either as a string to the file path or they can send over the encoded image string.

The unit tests I did via Postman.
First run the solution in Visual Studio.
After the solution is running, click the below link to access the Postman Unit Tests:

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/fe39bd6d92ffa504d035)

(See also file ContactManagerUnitTests.postman_collection.json)

For each test request under the collection ContactManagerUnitTests, click "send"

The tab called "Test Results" will show the results of the tests.

The "View Profile Bytes" test must be run after the "Put Contact" test because the image byte string is added via the Put request
