DROP TABLE IF EXISTS `Prescriptions`;
DROP TABLE IF EXISTS `DoctorUser`;
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
  `SingleUseCode` varchar(44) NOT NULL,
  `CreationDate` datetime NOT NULL,
  `IsFree` tinyint(1) NOT NULL,
  `DrugName` varchar(12) NOT NULL,
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

INSERT INTO `Users` (`Id`, `FirstName`, `LastName`, `Email`, `Password`, `Gender`, `Birthdate`, `Key2FA`, `Role`)
VALUES
  ('247e049a-0203-48fe-adfd-4110c21444ab','Paziente','Test','matteo.tossuto@gmail.com','$2a$11$4HODAPakSYyzIxKel71E4eFDtJ6byukWmcr3HJTkXdXV8hOk3Rwoe','Male','1800-01-01','sr2/X3KD/Cg=','patient'),
  ('3b7cd8a8-0dec-4974-8da2-9463848279d2','Davide','Albiero','davide@albiero.it','$2a$11$SiPGbD9E8O8nNiRAidiCouJogRP9zz3gsewIwAJAgZZeFxNQsi3q2','Male','1999-10-29','8qs4MpaRjeI=','doctor'),
  ('c110fcf5-3394-450e-9eb2-df101d0cfd44','Matteo','Tossuto','matteotossuto@gmail.com','$2a$11$vxslnl7.uDK.2Qlm.0d8De0R3YoYNodkaIIuSj/IbWevtaCuVkIc2','Other','1900-01-01','s7Hyto5f9EQ=','doctor'),
  ('d3d5dbf4-210d-41c8-a0f3-def05b498fd4','Tina','Anselmi','tina.anselmi@gmail.com','$2a$11$Ia/sf/yRlLVJa777afkrS.guKgjEwII9fb468MyxqKLW73qSEo97q','Female','1927-03-25','8L1MCdeuHC4=','patient');

INSERT INTO `Prescriptions` (`Id`, `IdDoctor`, `IdUser`, `IdPharmacist`, `SingleUseCode`, `CreationDate`, `IsFree`, `DrugName`)
VALUES
  ('41cf1a89-b9d7-4dcb-afcd-a810885bcf2e','3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4',NULL,'L7jEalJu5rXo6RV6nW38Ab03Koj9ZnG0zXubpB04qgE=','2023-05-17 16:50:37',0,'Paracetamol'),
  ('423afe04-7957-41df-888c-b349e4e29444','3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4',NULL,'9nOfi9UQDtzP3gm09HDLYsksqs1VgVaE+skCXlruSCs=','2023-05-20 12:55:06',0,'Ibuprofen'),
  ('937c05cf-f9a7-4c22-9296-092d0058337f','c110fcf5-3394-450e-9eb2-df101d0cfd44','d3d5dbf4-210d-41c8-a0f3-def05b498fd4',NULL,'8r+ZaZyk5bD9h0rgQ7TDEyEa0zlxsHsB2ypuuBrNU1k=','2023-05-22 18:13:30',1,'Test');

INSERT INTO `DoctorUser` (`IdDoctor`, `IdUser`)
VALUES
  ('3b7cd8a8-0dec-4974-8da2-9463848279d2','d3d5dbf4-210d-41c8-a0f3-def05b498fd4'),
  ('c110fcf5-3394-450e-9eb2-df101d0cfd44','247e049a-0203-48fe-adfd-4110c21444ab');
