Feature: DataBase
	In order make testing faster
	As a QA 
	I want the database to be checked automatically

	
Scenario: Create new person
	Given person data Ivan, Ivanov, 25, Kharkiv	
	When I send request to DB
	Then new user created with person data

	 
Scenario: Update person data
	Given new person data MIhail, Sotnikov, 34, Dnipro
	When I send request to DB
	Then new user created with person data

	
Scenario: Create order to last person
	Given order sum 10500
	When I send request to DB
	Then new order created

	
Scenario: Delete from orders last Person order 
	Given request for delete orders
	When I send request to DB
	Then order is deleted
		
	
	
Scenario: Delete last person
	Given request for delete person
	When I send request to DB
	Then person is deleted