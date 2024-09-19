-- Create stock table
create table stock (
    id int identity(1,1) primary key, -- Auto-increment primary key
    name varchar(255) ,
    price decimal(10, 2) ,
    quantity int 
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
create table sale (
    id int identity(1,1) primary key, -- Auto-increment primary key
    name varchar(255) not null,
    price decimal(10, 2) not null,
    quantity int not null,
    date datetime not null
);