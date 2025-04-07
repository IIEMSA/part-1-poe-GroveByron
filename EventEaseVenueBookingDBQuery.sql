-- CREATE VENUE TABLE
CREATE TABLE Venue (
VenueID INT IDENTITY (1,1) PRIMARY KEY,
VenueName NVARCHAR (150) NOT NULL,
VenueLocation NVARCHAR (150) NOT NULL,
VenueCapacity INT NOT NULL,
ImageURL NVARCHAR (255) NOT NULL
);

-- CREATE EVENT TABLE
CREATE TABLE Event (
EventID INT IDENTITY (1,1) PRIMARY KEY,
EventName NVARCHAR (150) NOT NULL,
EventDate DATE UNIQUE NOT NULL,
EventEndDate DATE NOT NULL,
EventDescription NVARCHAR (250) NOT NULL,
VenueID INT NOT NULL,
CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueID) REFERENCES Venue(VenueID)
);

-- CREATE BOOKING TABLE
CREATE TABLE Booking ( 
BookingID INT IDENTITY (1,1)UNIQUE,
BookingDate DATETIME NOT NULL, 
VenueID INT NOT NULL,
CONSTRAINT FK_Booking_Venue FOREIGN KEY (VenueID) REFERENCES Venue(VenueID),
EventID INT NOT NULL,
CONSTRAINT FK_Booking_Event FOREIGN KEY (EventID) REFERENCES Event(EventID)
);

-- INSERT SAMPLE DATA FOR VENUE
INSERT INTO Venue (VenueName, VenueLocation, VenueCapacity, ImageURL) 
VALUES 
('Aurora', 'Gauteng', 100, 'https://Aurora.co.za'),
('Avianto', 'KZN', 50, 'https://Avianto.co.za'),
('Bistro', 'North West', 35, 'https://Bistro.co.za'),
('Aragorn', 'Western Cape', 70, 'https://Aragorn.co.za');

-- INSERT SAMPLE DATA FOR EVENT
INSERT INTO Event (EventName, EventDate, EventEndDate, EventDescription, VenueID)
VALUES ('Golden Hour', '2025-01-15' ,'2025-01-16' ,'Golden Decorations with a twist of white confetti.', 1),
('Sunrise to Sundown', '2025-04-20' ,'2025-04-21' ,'White Masks with Orange and Yellow Decorations.', 2),
('Up Creek', '2025-03-23', '2025-03-24' ,'Green Decorations with a Green Balloon entrance gateway.', 3),
('Dusk til Dawn', '2025-07-12' , '2025-07-13' ,'Skull Decorations with Vibrant colour chairs and tables.', 4);

INSERT INTO Booking (BookingDate, VenueID, EventID)
VALUES 
('2025-01-15 09:00:00', 1, 1),  
('2025-04-20 10:00:00', 2, 2),  
('2025-03-23 13:00:00', 3, 3),  
('2025-07-12 17:00:00', 4, 4);  

-- Display values for Venue
SELECT * FROM Venue;
-- Display values for Event
SELECT * FROM Event;
-- Display values for Booking
SELECT * FROM Booking;

DROP TABLE Venue;
DROP TABLE Event;
DROP TABLE Booking;

ALTER TABLE Event DROP CONSTRAINT FK_Event_Venue;
ALTER TABLE Booking DROP CONSTRAINT FK_Booking_Venue;
ALTER TABLE Booking DROP CONSTRAINT FK_Booking_Event;

SELECT BookingDate FROM Booking;

SELECT CONVERT(VARCHAR, BookingDate, 120) AS BookingTime
FROM Booking;

