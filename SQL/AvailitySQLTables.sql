Use [AvailityDB]
Go

Create Schema [CSR];
Go

Create Schema [Security];
Go

Create User [AvaDeveloper] For Login [DeveloperAva]
	With Default_Schema = db_datareader;
Go

Create User [SQLBatch] For Login [ApplicationAva] 
	With Default_Schema = db_datawriter;
Go

Grant SELECT ON Schema::[CSR] To [AvaDeveloper];

Grant SELECT, INSERT, UPDATE, DELETE ON Schema::[CSR] To [SQLBatch];

Grant SELECT ON Schema::[Security] To [AvaDeveloper];

Grant SELECT, INSERT, UPDATE, DELETE ON Schema::[Security] To [SQLBatch];

Create Table [CSR].[Customer]
(
	[CustID] Int Identity(1,1) Primary Key Not Null,
	[FirstName] nvarchar(200),
	[LastName] nvarchar(300),	
	[CreateDate] [DateTime2] Not Null default( getdate() ),
	[ModifyDate] [DateTime2] default(getdate()),
	[ModifiedBy] Int
);

Create Table [CSR].[Order]
(
	[OrderID] Int Identity(1,1) Primary Key,
	[CustomerID] Int Foreign Key References [CSR].[Customer](CustID),
	[OrderDate] [DateTime2],
	[CreateDate] [DateTime2] Not Null default( getdate() ),
	[ModifyDate] [DateTime2] default(getdate()),
	[ModifiedBy] Int
);

Create Table [CSR].[OrderLine]
(
	[OrderLineID] Int Identity(1,1) Primary Key,
	[OrdID] Int Foreign Key References [CSR].[Order](OrderID),
	[ItemName] nvarchar(300),
	[Cost] money,
	[Quantity] Int,	
	[CreateDate] [DateTime2] Not Null default( getdate() ),
	[ModifyDate] [DateTime2] default(getdate()),
	[ModifiedBy] Int
);

Create Table [CSR].[ClientList]
(
	[ClientID] Int Identity(1,1) Primary Key Not Null,
	[Company] Nvarchar( 125 ),
	[NPINumber] Int Not Null,
	[ClientType] Nvarchar(65),
	[CreateDate] Datetime2(0) default(CURRENT_TIMESTAMP)
);

Create Table [CSR].[Company]
(
	[COMPID] [int] IDENTITY(1,1) Primary Key NOT NULL,
	[Status] Int Not Null default(1),
	[DateEstablished] [datetime] NULL,
	[Company] Nvarchar(125) NULL,
	[WebUrl] Nvarchar(125) NULL,
	[BusinessAddress] Nvarchar(300) Not Null,
	[Phone] Nvarchar(15) Not Null,
	[NPINumber] Int Not Null,
	[CreateDate] Datetime2(0) default(CURRENT_TIMESTAMP)
);

Create Table [Security].[Users]
(
	[UserID] Int IDENTITY(1,1) Primary Key NOT NULL,
	[FirstName] Nvarchar(125) Not Null,
	[MiddleName] Nvarchar(100) Null default(''),
	[LastName] Nvarchar(125) Not Null,
	[LoginName] Nvarchar(200) Not Null,
	[Password] Binary(64) Not Null,
	[Title] Varchar(200) NULL,
	[AKA] Varchar(90) NULL,
	[Suffix] Nvarchar(12) NULL,
	[Status] Int NOT NULL default(1),
	[Company] Int Foreign Key References [CSR].[Company](CompID),
	[CreateDate] Datetime2(0) default(CURRENT_TIMESTAMP)
);

Create Table [Security].[Registration]
(
	[RegID] Int Identity(1,1) Primary Key,
	[UserID] Int Foreign Key References [Security].[Users](UserID),
	[CreateDate] Datetime2(0) default(CURRENT_TIMESTAMP)
);

Select [CustID], 
	   [FirstName], 
	   [LastName], 
	   CONVERT(DATETIME2(0), [CreateDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [CreateDate_TZ_EST],
	   CONVERT(DATETIME2(0), [ModifyDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [Modified_TZ_EST],
	   Case When [ModifiedBy] = 34 Then 'Walker' Else '' End As [RecordManager]
	From [CSR].[Customer] with(nolock);

Update [CSR].[Customer]
	Set ModifiedBy = 34,
	[ModifyDate] = CURRENT_TIMESTAMP 
	Where CustID = 1


/****** Object:  StoredProcedure [CSR].[sp_OrderFulfillment]    Script Date: 8/17/2013 7:18:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create Procedure [CSR].[sp_OrderFulfillment]
	@CustID int,
	@ItemName nvarchar( 125 ) = null,
	@Cost money = null,
	@Quantity int = null
As
Declare @ReturnIdentity int;

Begin Try  
		IF Exists( Select [CustID] From [CSR].[Customer] with(nolock) Where ( [CustID] = @CustID ) )
			Begin Tran	
					Insert Into [CSR].[Order] ( [CustomerID], [OrderDate] )
								Values ( @CustID, CURRENT_TIMESTAMP );
					
							Set @ReturnIdentity = SCOPE_IDENTITY();

					Insert Into [CSR].[OrderLine] ( [OrdID], [ItemName], [Cost], [Quantity] )
						Values ( @ReturnIdentity, @ItemName, @Cost, @Quantity);
			Commit Tran
End Try

Begin Catch
	IF ( XACT_STATE() ) = -1	
			ROLLBACK TRANSACTION 

		DECLARE @ErrorMessage	nvarchar(4000),
				@ErrorSeverity	int,
				@ErrorState		int;

		SELECT	@ErrorMessage	=	ERROR_MESSAGE(),
				@ErrorSeverity	=	ERROR_SEVERITY(),
				@ErrorState		=	ERROR_STATE();

		RAISERROR ( @ErrorMessage, @ErrorSeverity, @ErrorState );
End Catch
	Set Nocount Off;
Go

Create Procedure [Security].[sp_Register_ClientInfo]
	@LoginName Nvarchar( 165 ),
	@NPINumber Int,
	@FullName Nvarchar( 355 ),
	@Password Binary( 64 ),
	@BusinessAddress Nvarchar( 265 ),
	@Telephone Nvarchar( 15 ),
	@ReturnADIdentity int out
As
	Set Nocount On;
Begin Try  
		IF Exists( Select [UserID] From [Security].[Users] As U with(nolock) Left Join [CSR].[Company] As C with(nolock) On U.[Company] = C.[CompID] Where ( [LoginName] = @LoginName And [NPINumber] = @NPINumber ) )
			Begin
				Select 'This Login name and other registration information is already registered to this NPI Number.'
			End
		Else
			Begin
				Declare @Company Nvarchar(115), @CompIdentity Int, @UserIdentity Int, @First Nvarchar(125), @Middle Nvarchar(100), @Last Nvarchar(125);

				Set @First = (Select value From STRING_SPLIT(@FullName, ' ', 1) Where [Ordinal] = 1);

				If ((Select Count(value)  From STRING_SPLIT(@FullName, ' ', 1)) > 2) 
				Begin
					Set @Middle = (Select value From STRING_SPLIT(@FullName, ' ', 1) Where [Ordinal] = 2);
					Set @Last = (Select value From STRING_SPLIT(@FullName, ' ', 1) Where [Ordinal] = 3);
				End
				Else
				Begin
					Set @Last = (Select value From STRING_SPLIT(@FullName, ' ', 1) Where [Ordinal] = 2);
				End			

				Set @Company = (Select [Company] From [CSR].[ClientList] with(nolock) Where [NPINumber] = @NPINumber);

				Begin Tran	
					Insert Into [CSR].[Company] ( [Company], [BusinessAddress], [Phone], [NPINumber] )
						Values ( @Company, @BusinessAddress, @Telephone, @NPINumber )
					
					Set @CompIdentity = SCOPE_IDENTITY();

					Insert Into [Security].[Users] ( [FirstName], [MiddleName], [LastName], [LoginName], [Password], [Company] )
						Values ( @First, @Middle, @Last, @LoginName, @Password, @CompIdentity );

					Set @UserIdentity = SCOPE_IDENTITY();

					Insert Into [Security].[Registration] ( [UserID] )
						Values( @UserIdentity );

						Set @ReturnADIdentity = SCOPE_IDENTITY();
				Commit Tran

				return @ReturnADIdentity;
			End
End Try

Begin Catch
	IF ( XACT_STATE() ) = -1	
			ROLLBACK TRANSACTION 

		DECLARE @ErrorMessage	nvarchar(4000),
				@ErrorSeverity	int,
				@ErrorState		int;

		SELECT	@ErrorMessage	=	ERROR_MESSAGE(),
				@ErrorSeverity	=	ERROR_SEVERITY(),
				@ErrorState		=	ERROR_STATE();

		RAISERROR ( @ErrorMessage, @ErrorSeverity, @ErrorState );
End Catch
	Set Nocount Off;
Go

Declare @LoginName Nvarchar( 165 ),
	@NPINumber Int,
	@FullName Nvarchar( 355 ),
	@Password Binary( 64 ),
	@BusinessAddress Nvarchar( 265 ),
	@Telephone Nvarchar( 15 ),
	@ReturnADIdentity int;

Set @LoginName = 'fred.david.winter.InternalMedicine@gmail.com'; Set @NPINumber = 1609889963; Set @FullName = 'Fred David Winter'; Set @Password = Cast('klajsdfj!3iojaf09j3mf309j$#LJikj3jkL#' As Binary); Set @BusinessAddress = '3434 SWISS AVE SUITE 105 DALLAS, TX 75204-6251'; Set @Telephone = '(214)-828-0010';

Execute [Security].[sp_Register_ClientInfo]
	@LoginName,
	@NPINumber,
	@FullName,
	@Password,
	@BusinessAddress,
	@Telephone,
	@ReturnADIdentity out;

Select *
	From [Security].[Users] As U with(nolock)
	Inner Join [Security].[Registration] As Reg with(nolock) On U.[UserID] = Reg.[UserID]
	Left Join [CSR].[Company] As C with(nolock) On U.[Company] = C.[CompID]

Execute [CSR].[sp_OrderFulfillment] 2, 'Healthcare Service Gateway Elite Provider option 2', 1200, 1;
Execute [CSR].[sp_OrderFulfillment] 10, 'NPI Checker Service Gateway option 1', 100, 4;
Execute [CSR].[sp_OrderFulfillment] 18, 'API Restful services', 2934, 11;
Execute [CSR].[sp_OrderFulfillment] 4, 'Service Gateway Elite Provider option 1', 1800, 8;
Execute [CSR].[sp_OrderFulfillment] 7, 'SOAP Services for EDI management', 126200, 1;
Execute [CSR].[sp_OrderFulfillment] 16, 'Service Gateway Elite Provider option 1', 1800, 28;
Execute [CSR].[sp_OrderFulfillment] 13, 'API Restful services', 2934, 9;


/*
	a.	Write a SQL query that will produce a reverse-sorted list (alphabetically by name) of customers (first and last names) whose last name begins with the letter ‘S.’
	Here is response to request A...TCDW by T. Christopher D. Walker
*/
Select [CustID], 
	   [FirstName], 
	   [LastName], 
	   CONVERT(DATETIME2(0), C.[CreateDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [CreateDate_TZ_EST],
	   CONVERT(DATETIME2(0), C.[ModifyDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [Modified_TZ_EST],
	   Case When C.[ModifiedBy] = 34 Then 'Walker' Else '' End As [RecordManager],
	   O.[OrderID],
	   O.[CustomerID] As [Order_CustomerID],
	   CONVERT(DATETIME2(0), O.[OrderDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [OrderDate_TZ_EST],
	   OL.[OrderLineID],
	   OL.[ItemName],
	   OL.[Cost],
	   OL.[Quantity]
	From [CSR].[Customer] As C with(nolock) 
	Inner Join [CSR].[Order] As O with(nolock) On C.[CustID] = O.[CustomerID]
	Inner Join [CSR].[OrderLine] As OL with(nolock) On O.[OrderID] = OL.[OrdID]
	Where [LastName] Like 'S%'
	Order By [FirstName], [LastName] DESC;

	--Below are the results from above query...TCDW


/*
	b.	Write a SQL query that will show the total value of all orders each customer has placed in the past six months. Any customer without any orders should show a $0 value.
	Here is response to request B answered in two ways...TCDW by T. Christopher D. Walker
*/
	Select [CustID], 
	   [FirstName], 
	   [LastName], 
	   O.[OrderID],
	   O.[CustomerID] As [Order_CustomerID],
	   CONVERT(DATETIME2(0), O.[OrderDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [OrderDate_TZ_EST],
	   OL.[OrderLineID],
	   OL.[ItemName],
	   '$' + CAST(format((OL.[Cost] * OL.[Quantity]), '#,##0.00') As NVARCHAR(100)) As [TotalValue_EachCustomer]
	From [CSR].[Customer] As C with(nolock) 
	Inner Join [CSR].[Order] As O with(nolock) On C.[CustID] = O.[CustomerID]
	Inner Join [CSR].[OrderLine] As OL with(nolock) On O.[OrderID] = OL.[OrdID]
	Where O.[OrderDate]  >= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, CURRENT_TIMESTAMP)), 0)
	Group By [CustID], [FirstName], [LastName], O.[OrderID], O.[CustomerID], O.[OrderDate], OL.[OrderLineID], OL.[ItemName], OL.[Cost], OL.[Quantity];
	
	--Here is total value with all orders together...TCDW
	Select 'Total Value' As [All Orders Total],
	   '$' + CAST(format(SUM((OL.[Cost] * OL.[Quantity])), '#,##0.00') As NVARCHAR(100)) As [TotalValue_EachCustomer]
	From [CSR].[Customer] As C with(nolock) 
	Inner Join [CSR].[Order] As O with(nolock) On C.[CustID] = O.[CustomerID]
	Inner Join [CSR].[OrderLine] As OL with(nolock) On O.[OrderID] = OL.[OrdID]
	Where O.[OrderDate]  >= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, CURRENT_TIMESTAMP)), 0)	

	--Below are the results from above query...TCDW


	/*
		c.	Amend the query from the previous question to only show those customers who have a total order value of more than $100 and less than $500 in the past six months.
		Here is response to request C...TCDW by T. Christopher D. Walker
	*/
	Select [CustID], 
	   [FirstName], 
	   [LastName], 
	   O.[OrderID],
	   O.[CustomerID] As [Order_CustomerID],
	   CONVERT(DATETIME2(0), O.[OrderDate] AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time') As [OrderDate_TZ_EST],
	   OL.[OrderLineID],
	   OL.[ItemName],
	   '$' + CAST(format((OL.[Cost] * OL.[Quantity]), '#,##0.00') As NVARCHAR(100)) As [TotalValue_EachCustomer]
	From [CSR].[Customer] As C with(nolock) 
	Inner Join [CSR].[Order] As O with(nolock) On C.[CustID] = O.[CustomerID]
	Inner Join [CSR].[OrderLine] As OL with(nolock) On O.[OrderID] = OL.[OrdID]
	Where O.[OrderDate]  >= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, CURRENT_TIMESTAMP)), 0)
	AND
	(OL.[Cost] * OL.[Quantity]) Between 100 And 500
	Group By [CustID], [FirstName], [LastName], O.[OrderID], O.[CustomerID], O.[OrderDate], OL.[OrderLineID], OL.[ItemName], OL.[Cost], OL.[Quantity];

	--Below are teh results from above query...TCDW
