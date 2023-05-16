DROP TABLE IF EXISTS `Prescriptions`;
DROP TABLE IF EXISTS `Users`;

CREATE TABLE `Users` (
  `Id` varchar(40) NOT NULL,
  `FirstName` varchar(30) NOT NULL,
  `LastName` varchar(30) NOT NULL,
  `Email` varchar(30) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Gender` varchar(6) NOT NULL COMMENT 'male/female/other',
  `Birthdate` date NOT NULL,
  `Key2FA` varchar(12) NOT NULL,
  `Role` varchar(10) NOT NULL COMMENT 'doctor/patient/pharmacist',
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `Prescriptions` (
  `Id` varchar(40) NOT NULL,
  `IdDoctor` varchar(40) NOT NULL,
  `IdUser` varchar(40) NOT NULL,
  `IdPharmacist` varchar(40),
  `SingleUseCode` varchar(40) NOT NULL,
  `CreationDate` date NOT NULL,
  `IsFree` tinyint(1) NOT NULL,
  `DrugName` varchar(12) NOT NULL,
  PRIMARY KEY(`Id`),
  FOREIGN KEY (`IdDoctor`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IdUser`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IdPharmacist`) REFERENCES `Users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
