CREATE TABLE Business (

	business_id 		VARCHAR(50) PRIMARY KEY,
	business_name 		VARCHAR(250) NOT NULL,
	city				VARCHAR(50) NOT NULL,
	business_state		VARCHAR(50) NOT NULL,
	zipcode				INTEGER NOT NULL,
	lat					FLOAT NOT NULL,
	longi				FLOAT NOT NULL,
	business_address	VARCHAR(250) NOT NULL,
	review_count		INTEGER DEFAULT 0,
	is_open				BOOLEAN DEFAULT FALSE,
	stars				FLOAT DEFAULT 0,
	numCheckins			INTEGER DEFAULT 0
);

CREATE TABLE Customer ( -- need to store user's friends and favorites
	
	user_id 		VARCHAR(50) PRIMARY KEY,
	cool			INTEGER DEFAULT 0,
	funny			INTEGER DEFAULT 0,
	useful			INTEGER DEFAULT 0,
	cust_name		VARCHAR(75) NOT NULL,
	fans			INTEGER DEFAULT 0,
	review_count	INTEGER DEFAULT 0,
	yelping_since	VARCHAR(15),
	user_lat		FLOAT DEFAULT NULL,
	user_longi		FLOAT DEFAULT NULL
);

CREATE TABLE Friends (
	
	friend_id		VARCHAR(50),
	user_id			VARCHAR(50),
	FOREIGN KEY (user_id) REFERENCES Customer(user_id)
);
	
CREATE TABLE Review (

	rev_id 			VARCHAR(50) PRIMARY KEY,
	user_id			VARCHAR(50) NOT NULL,
	business_id		VARCHAR(50) NOT NULL,
	rev_stars		INTEGER NOT NULL,
	rev_date		VARCHAR(10),
	rev_text		TEXT NOT NULL,
	useful_count	INTEGER DEFAULT 0,
	funny_count		INTEGER DEFAULT 0,
	cool_count		INTEGER DEFAULT 0,
	FOREIGN KEY (business_id) REFERENCES Business(business_id),
	FOREIGN KEY (user_id) REFERENCES Customer(user_id)
);

CREATE TABLE Categories (

	business_id 		VARCHAR(50),
	cat_name		VARCHAR(50),
	PRIMARY KEY (business_id, cat_name),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE BusinessHours (

	business_id		VARCHAR(50),
	hr_day			VARCHAR(9),
	hr_close		VARCHAR(10) NOT NULL,
	hr_open			VARCHAR(10) NOT NULL,
	PRIMARY KEY(business_id, hr_day),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Checkins (

	business_id		VARCHAR(50), 
	ch_day			VARCHAR(9),
	ch_time			VARCHAR(10),
	ch_count		INTEGER DEFAULT 0,
	PRIMARY KEY (business_id, ch_day, ch_time)
);

CREATE TABLE Favorites (
	business_id		VARCHAR(50),
	user_id			VARCHAR(50),
	PRIMARY KEY(business_id, user_id),
	FOREIGN KEY (business_id) REFERENCES Business(business_id),
	FOREIGN KEY (user_id) REFERENCES Customer(user_id)
);
/*
	If you have to wipe your database a lot like me, and it won't b/c there's connections on it:
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'yelpdb3';
*/

