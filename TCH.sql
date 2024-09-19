-- Create stock table
CREATE TABLE stock (
    id INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-increment primary key
    name VARCHAR(255) ,
    price DECIMAL(10, 2),
    quantity INT ,
    gender VARCHAR(50),  -- New column for gender
    color VARCHAR(50),    -- New column for color
	date datetime ,
	total decimal(18,0) 
);

-- Create defected table
create table defected (
    id int identity(1,1) primary key, -- Auto-increment primary key
    name varchar(255) not null,
    date datetime not null,
    price decimal(10, 2) not null,
    problem varchar(255),
    status varchar(50)
);

-- Create sale table
CREATE TABLE sale (
    ID INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-incremented ID for the sale
    name VARCHAR(255) NOT NULL,        -- Name of the product sold
    customername VARCHAR(255),         -- Name of the customer
    price DECIMAL(10, 2) NOT NULL,     -- Selling price of the product
    quantity INT NOT NULL,             -- Quantity sold
    total DECIMAL(10, 2) NOT NULL,     -- Total sale amount (price * quantity)
    profit DECIMAL(10, 2) NOT NULL,    -- Profit from the sale (calculated as (sell price - purchase price) * quantity)
    date DATETIME NOT NULL             -- Date and time of the sale
);

alter table sale 
expenseP DECIMAL(10, 2)
create table investor(
id int identity(1,1) primary key,
Investorname nvarchar(50) not null,
amount decimal not null,
);


create table login (
id int identity(1,1) primary key,
email nvarchar(50) not null,
password nvarchar(50) not null,
type nvarchar(50) not null,
);


create table profitDistribution (
id int identity(100,1) primary key,
amount decimal not null,
pdate date not null,
);

create table itemreturn (
id int identity(100,1) primary key,
itemname nvarchar(50) not null,
price decimal (10,2) not null,
quantity int not null,
rdate date not null
);

alter table itemreturn 

