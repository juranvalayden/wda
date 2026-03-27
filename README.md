## WDA | The Assessment
1. Clone the solution.

### Powershell cmd
1. Once the solution is cloned, in the root folder enter the following cmd: docker compose up

### Unit Tests
1. UserController tests
2. AuthenticationController tests

### Postman
1. I have added a postman collection to test the API for the positive and negative test cases
2. You may need to change the port or use the environment variable within the collection itself

### Viewing the endpoints via Scalar
1. Once docker compose is up and running the scalar endpoint port 8080
2. To view the users: http://localhost:8080/scalar/v1
3. Only authorization using JWT tokens that are valid for an hour, allows you to view the user details

### No Frontend, Hmm, that is curious
1. I didn't quite get around to creating this part, hence the postman collection
