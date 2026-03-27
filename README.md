## WDA | The Assessment
1. Clone the solution.

### Powershell cmd
1. Once the solution is cloned, in the root folder enter the following cmd: docker compose up

### Unit Tests
1. UserController tests
2. AuthenticationController tests

### Postman
1. I have added a postman collection to test the API for the positive and negative test cases
2. You may need to change the port based on whether you're running the in docker(8080) or locally(7146)
3. The collection has a variable, aptly named {{portNumber}}
4. The location of the collection can be imported from the "Solution Items" folder

### Viewing the endpoints via Scalar
1. Once docker compose is up and running the scalar endpoint port 8080
2. To view the users: http://localhost:8080/scalar/v1
3. Only upon successful authorization aka login will you get a JWT token
4. The tokens are valid for an hour
5. It allows you to view the user details via email retrieval

### No Frontend, Hmm, that is curious
1. I didn't quite get around to creating this part, hence the postman collection
