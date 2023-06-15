DROP TABLE IF EXISTS `Prescriptions`;
DROP TABLE IF EXISTS `DoctorUser`;
DROP TABLE IF EXISTS `Notifications`;
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
  `CreationDate` datetime NOT NULL,
  `IsFree` tinyint(1) NOT NULL,
  `DrugName` varchar(100) NOT NULL,
  PRIMARY KEY(`Id`),
  FOREIGN KEY (`IdDoctor`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IdUser`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IdPharmacist`) REFERENCES `Users` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `DoctorUser` (
  `IdDoctor` varchar(40) NOT NULL,
  `IdUser` varchar(40) NOT NULL,
  PRIMARY KEY(`IdDoctor`, `IdUser`),
  FOREIGN KEY (`IdDoctor`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IdUser`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `Notifications` (
  `IDUser` varchar(40) NOT NULL,
  `Url` varchar(500) NOT NULL,
  `P256dh` varchar(255) NOT NULL,
  `Auth` varchar(255) NOT NULL,
  `CreationDate` datetime NOT NULL,
  PRIMARY KEY(`IDUser`, `Url`),
  FOREIGN KEY (`IDUser`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `Users` (`Id`, `FirstName`, `LastName`, `Email`, `Password`, `Gender`, `Birthdate`, `Key2FA`, `Role`)
VALUES
  ('247e049a-0203-48fe-adfd-4110c21444ab','Paziente','Test','matteo.tossuto@gmail.com','$2a$11$4HODAPakSYyzIxKel71E4eFDtJ6byukWmcr3HJTkXdXV8hOk3Rwoe','Male','1800-01-01','sr2/X3KD/Cg=','patient'),
  ('3b7cd8a8-0dec-4974-8da2-9463848279d2','Davide','Albiero','davide@albiero.it','$2a$11$SiPGbD9E8O8nNiRAidiCouJogRP9zz3gsewIwAJAgZZeFxNQsi3q2','Male','1999-10-29','8qs4MpaRjeI=','doctor'),
  ('c110fcf5-3394-450e-9eb2-df101d0cfd44','Matteo','Tossuto','matteotossuto@gmail.com','$2a$11$vxslnl7.uDK.2Qlm.0d8De0R3YoYNodkaIIuSj/IbWevtaCuVkIc2','Other','1900-01-01','s7Hyto5f9EQ=','doctor'),
  ('d3d5dbf4-210d-41c8-a0f3-def05b498fd4','Tina','Anselmi','tina.anselmi@gmail.com','$2a$11$Ia/sf/yRlLVJa777afkrS.guKgjEwII9fb468MyxqKLW73qSEo97q','Female','1927-03-25','8L1MCdeuHC4=','patient'),
  ('816cb763-8484-4d68-a1a5-0e061d73e56f','Dottor','House','house@gmail.com','$2a$11$Kr6v4.LKLCyRaOiGFILJ2u9q2GlidXakXSjgnluXj2KJPpV31Y8Z.','Male','1500-01-01','Ss8PqYwNcvo=','pharmacist'),
  ('c2743318-563b-44d3-9410-e903e7afdbb0','Mario','Rossi','mario.rossi@gmail.com','$2a$11$KnnNoR7yy4GBzWB.nmUcxeOavdI0BWts.7mpVF4gqo5AyfIu2pkxy','Male','1980-04-20','RiWnWaLx2KU=','pharmacist');

INSERT INTO `Prescriptions` (`Id`, `IdDoctor`, `IdUser`, `IdPharmacist`, `SingleUseCode`, `CreationDate`, `IsFree`, `DrugName`)
VALUES
  ('41cf1a89-b9d7-4dcb-afcd-a810885bcf2e','3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4',NULL,'4a439d9f-43b8-49c3-bcf5-253d84c905a1','2023-05-17 16:50:37',0,'Paracetamol'),
  ('423afe04-7957-41df-888c-b349e4e29444','3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4',NULL,'df2002b8-1bf8-45f5-89b6-f87d0f2639aa','2023-05-20 12:55:06',0,'Ibuprofen');

INSERT INTO `DoctorUser` (`IdDoctor`, `IdUser`)
VALUES
  ('3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4'),
  ('c110fcf5-3394-450e-9eb2-df101d0cfd44','247e049a-0203-48fe-adfd-4110c21444ab');
