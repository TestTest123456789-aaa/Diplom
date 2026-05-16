-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: bpr
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bpr_info`
--

DROP TABLE IF EXISTS `bpr_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpr_info` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Date` varchar(45) DEFAULT NULL,
  `Time` varchar(45) DEFAULT NULL,
  `Count_Students` varchar(45) DEFAULT NULL,
  `Responsible_user` int DEFAULT NULL,
  `Lesson` int DEFAULT NULL,
  `GroupId` int DEFAULT NULL,
  `RoomId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `teachers_idx` (`Responsible_user`),
  KEY `lesson_idx` (`Lesson`),
  KEY `fk_bpr_group` (`GroupId`),
  KEY `RoomId` (`RoomId`),
  CONSTRAINT `bpr_info_ibfk_1` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_bpr_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `lessons` FOREIGN KEY (`Lesson`) REFERENCES `lessons` (`Id`),
  CONSTRAINT `teachers` FOREIGN KEY (`Responsible_user`) REFERENCES `teachers` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bpr_info`
--

LOCK TABLES `bpr_info` WRITE;
/*!40000 ALTER TABLE `bpr_info` DISABLE KEYS */;
INSERT INTO `bpr_info` VALUES (1,'21.05.2026','10:00-11:30','3',1,3,1,1),(2,'19.05.2026','09:05 - 10:10','2',1,1,2,1),(4,'19.05.2026','08:00 - 09:30','5',2,2,6,6),(13,'23.05.2026','13:10 - 14:30','5',20,2,2,3),(14,'20.05.2026','09:50 - 10:55','0',4,4,1,5),(15,'25.05.2026','12:30 - 13:45','0',11,5,1,5),(17,'27.05.2026','15:25 - 16:30','7',25,12,1,4),(18,'15.05.2026','08:00 - 09:30','0',2,1,2,6);
/*!40000 ALTER TABLE `bpr_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bpr_participants`
--

DROP TABLE IF EXISTS `bpr_participants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpr_participants` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `BPR_Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `Nomer_paketa` varchar(45) DEFAULT NULL,
  `Kod` varchar(45) DEFAULT NULL,
  `Otmetka_ov_otsytstvii` varchar(45) DEFAULT NULL,
  `Sredniy_ball_attestata` varchar(45) DEFAULT NULL,
  `Otsenka_po_russkomy_yaziky` varchar(45) DEFAULT NULL,
  `Otsenka_po_matematike` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_participant_bpr` (`BPR_Id`),
  KEY `fk_participant_student` (`StudentId`),
  CONSTRAINT `fk_participant_bpr` FOREIGN KEY (`BPR_Id`) REFERENCES `bpr_info` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_participant_student` FOREIGN KEY (`StudentId`) REFERENCES `student_info` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bpr_participants`
--

LOCK TABLES `bpr_participants` WRITE;
/*!40000 ALTER TABLE `bpr_participants` DISABLE KEYS */;
INSERT INTO `bpr_participants` VALUES (1,1,1,'5225','11111','Присутствовал','5','5','4'),(2,1,3,'5527','5523132','Присутствовал','4.57','4','5'),(3,1,5,'5229','5403','Отсутствовал','4.23','5','4'),(4,2,2,'5522','12345','Присутствовал','5','5','5'),(5,2,4,'5528','4424','Присутствовал','4.6','3','5');
/*!40000 ALTER TABLE `bpr_participants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `departments`
--

DROP TABLE IF EXISTS `departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `departments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) DEFAULT NULL,
  `HeadTeacherId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_dept_head` (`HeadTeacherId`),
  CONSTRAINT `fk_dept_head` FOREIGN KEY (`HeadTeacherId`) REFERENCES `teachers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `departments`
--

LOCK TABLES `departments` WRITE;
/*!40000 ALTER TABLE `departments` DISABLE KEYS */;
INSERT INTO `departments` VALUES (1,'Вычислительная техника',3),(2,'Экономика и право',2),(4,'Строительство и архитектура',4),(5,'Сфера обслуживания',10);
/*!40000 ALTER TABLE `departments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `groups` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  `HeadTeacherId` int DEFAULT NULL,
  `DepartmentId` int DEFAULT NULL,
  `SpecialtyId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_head_teacher` (`HeadTeacherId`),
  KEY `fk_group_dept` (`DepartmentId`),
  KEY `fk_group_spec` (`SpecialtyId`),
  CONSTRAINT `fk_group_dept` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_group_spec` FOREIGN KEY (`SpecialtyId`) REFERENCES `specialties` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_head_teacher` FOREIGN KEY (`HeadTeacherId`) REFERENCES `teachers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groups`
--

LOCK TABLES `groups` WRITE;
/*!40000 ALTER TABLE `groups` DISABLE KEYS */;
INSERT INTO `groups` VALUES (1,'ИСП-21-4',7,1,1),(2,'ИСП-21-2',3,1,1),(4,'КС-21-2',8,1,1),(5,'КС-21-3',2,1,1),(6,'АС-21-2',4,1,1),(7,'АС-22-1',6,1,1);
/*!40000 ALTER TABLE `groups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lessons`
--

DROP TABLE IF EXISTS `lessons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lessons` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Lesson` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lessons`
--

LOCK TABLES `lessons` WRITE;
/*!40000 ALTER TABLE `lessons` DISABLE KEYS */;
INSERT INTO `lessons` VALUES (1,'Математика'),(2,'Русский язык'),(3,'Химия'),(4,'Физика'),(5,'Информатика'),(9,'География'),(10,'Английский язык'),(11,'История'),(12,'Физкультура'),(13,'Основы права'),(14,'Базы данных');
/*!40000 ALTER TABLE `lessons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `password_recovery`
--

DROP TABLE IF EXISTS `password_recovery`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `password_recovery` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(100) NOT NULL,
  `RecoveryCode` varchar(10) NOT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `Used` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `idx_email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `password_recovery`
--

LOCK TABLES `password_recovery` WRITE;
/*!40000 ALTER TABLE `password_recovery` DISABLE KEYS */;
INSERT INTO `password_recovery` VALUES (1,'colovok@gmail.com','579118','2026-05-07 16:47:05',0),(2,'colovok@gmail.com','788472','2026-05-07 16:50:05',0),(3,'colovok@gmail.com','846560','2026-05-07 17:05:34',0),(4,'colovok@gmail.com','745481','2026-05-07 17:11:54',0),(5,'colovok@gmail.com','243467','2026-05-07 17:31:35',0),(6,'colovok@gmail.com','508569','2026-05-07 17:44:29',0);
/*!40000 ALTER TABLE `password_recovery` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Capacity` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
INSERT INTO `rooms` VALUES (1,'А402',30),(2,'А403',25),(3,'А405',30),(4,'А409',20),(5,'Б414',30),(6,'Б418',15),(7,'В423',25),(8,'А301',35),(9,'А305',28),(10,'Б310',40);
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `specialties`
--

DROP TABLE IF EXISTS `specialties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `specialties` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` varchar(20) DEFAULT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `DepartmentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_spec_dept` (`DepartmentId`),
  CONSTRAINT `fk_spec_dept` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `specialties`
--

LOCK TABLES `specialties` WRITE;
/*!40000 ALTER TABLE `specialties` DISABLE KEYS */;
INSERT INTO `specialties` VALUES (1,'09.02.07','Информационные системы и программирование',1),(2,'38.02.01','Экономика и бухгалтерский учет',2),(11,'10.02.05','Обеспечение информационной безопасности',1),(12,'40.02.01','Право и организация социального обеспечения',2);
/*!40000 ALTER TABLE `specialties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `student_info`
--

DROP TABLE IF EXISTS `student_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `student_info` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nomer_paketa` varchar(45) DEFAULT NULL,
  `Pol` varchar(45) DEFAULT NULL,
  `Kod` varchar(45) DEFAULT NULL,
  `FIO` varchar(45) DEFAULT NULL,
  `Otmetka_ov_otsytstvii` varchar(45) DEFAULT NULL,
  `Sredniy_ball_attestata` varchar(45) DEFAULT NULL,
  `Otsenka_po_russkomy_yaziky` varchar(45) DEFAULT NULL,
  `Otsenka_po_matematike` varchar(45) DEFAULT NULL,
  `Forma_obychenya` varchar(45) DEFAULT NULL,
  `Group_name` int DEFAULT NULL,
  `Spisok_BPR_Id` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Spisok_BPR_id_idx` (`Spisok_BPR_Id`),
  KEY `Groups_Id_idx` (`Group_name`),
  CONSTRAINT `Group_Id` FOREIGN KEY (`Group_name`) REFERENCES `groups` (`Id`),
  CONSTRAINT `Spisok_BPR_id` FOREIGN KEY (`Spisok_BPR_Id`) REFERENCES `bpr_info` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=75 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `student_info`
--

LOCK TABLES `student_info` WRITE;
/*!40000 ALTER TABLE `student_info` DISABLE KEYS */;
INSERT INTO `student_info` VALUES (1,'5225','Мужской','11111','Нецветаев Сергей Константинович','Присутствовал','5','5','4','Заочная',1,1),(2,'5522','Мужской','12345','Новиков Тимофей Сергеевич','Присутствовал','5','5','5','Очная',1,2),(3,'5527','Мужской','5523132','Осипов Роман Александрович','Присутствовал','4.57','4','5','Очная',1,1),(4,'5528','Мужской','4424','Пешков Даниил Викторович','Присутствовал','4.6','3','5','Очная',1,2),(5,'5229','Мужской','5403','Чадаев Михаил Алексеевич','Отсутствовал','4.23','5','4','Очная',2,1),(9,'','Мужской','','Нецветаев Сергей Константинович','','','','','Заочная',1,13),(10,'','Мужской','','Новиков Тимофей Сергеевич','','','','','Очная',1,13),(11,'','Мужской','','Осипов Роман Александрович','','','','','Очная',1,13),(12,'','Мужской','','Пешков Даниил Викторович','','','','','Очная',1,13),(14,'','Мужской','','Нецветаев Сергей Константинович','','','','','Заочная',1,14),(15,'','Мужской','','Новиков Тимофей Сергеевич','','','','','Очная',1,14),(16,'','Мужской','','Осипов Роман Александрович','','','','','Очная',1,14),(17,'','Мужской','','Пешков Даниил Викторович','','','','','Очная',1,14),(18,'','Мужской','','Кочетов Юрий Валерьевич','','','','','Очная',1,14),(39,'','Мужской','','Кочетов Юрий Валерьевич','','','','','Очная',1,15),(68,'','Мужской','','Кочетов Юрий Валерьевич','','','','','Очная',1,17),(69,'','Мужской','','Нецветаев Сергей Константинович','','','','','Очная',1,17),(70,'','Мужской','','Новиков Тимофей Сергеевич','','','','','Очная',1,17),(71,'','Мужской','','Чадаев Михаил Алексеевич','','','','','Очная',1,17),(72,'','Мужской','','Осипов Роман Александрович','','','','','Очная',1,17),(73,'','Женский','','Липина Кристина Александровна','','','','','Очная',1,17),(74,'','Мужской','','Пешков Даниил Викторович','','','','','Очная',1,17);
/*!40000 ALTER TABLE `student_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teachers`
--

DROP TABLE IF EXISTS `teachers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `teachers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FIO` varchar(45) DEFAULT NULL,
  `Login` varchar(45) DEFAULT NULL,
  `Parol` varchar(45) DEFAULT NULL,
  `Role` varchar(45) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `Phone_Number` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teachers`
--

LOCK TABLES `teachers` WRITE;
/*!40000 ALTER TABLE `teachers` DISABLE KEYS */;
INSERT INTO `teachers` VALUES (1,'Мишакина Ирина Леонидовна','login123','parol123','Преподаватель','ilmishakina@permavit.ru','89027461231'),(2,'Суслонова Мария Лазаревна','syslonova','Asdfg123','Преподаватель','syslonova@gmail.com','89028391231'),(3,'Куртагина Марина Владимировна','exlogin','exparol','Преподаватель','kurtagina@gmail.com','1234567'),(4,'Пример Пример Примерович','nets','123','Преподаватель','colovok@gmail.com','89048448804'),(5,'Администратор системы','example','example','Администратор','example','example'),(6,'Николаев Михаил Верещагин','testest1','testest','Преподаватель','testest@gmail.com','12345'),(7,'Ситчихин Олег Васильевич','sitx123','sitx123','Преподаватель','sitchihin@yandex.ru','213321'),(8,'Аристова Елена Геннадьевна','elena222','elena222','Преподаватель','aristova@gmail.com','3213215234'),(9,'Петров Алексей Иванович','petrov','petrov123','Преподаватель','petrov@college.ru','89001112233'),(10,'Сидорова Елена Дмитриевна','sidorova','sidorova456','Преподаватель','sidorova@college.ru','89002223344'),(11,'Козлов Михаил Сергеевич','kozlov','kozlov789','Преподаватель','kozlov@college.ru','89003334455'),(12,'Волкова Анна Павловна','volkova','volkova321','Преподаватель','volkova@college.ru','89004445566'),(13,'Морозов Дмитрий Александрович','morozov','morozov654','Преподаватель','morozov@college.ru','89005556677'),(16,'Козлов Михаил Сергеевич','kozlov','kozlov789','Преподаватель','kozlov@college.ru','89003334455'),(18,'Морозов Дмитрий Александрович','morozov','morozov654','Преподаватель','morozov@college.ru','89005556677'),(19,'Петров Алексей Иванович','petrov','petrov123','Преподаватель','petrov@college.ru','89001112233'),(20,'Сидорова Елена Дмитриевна','sidorova','sidorova456','Преподаватель','sidorova@college.ru','89002223344'),(21,'Козлов Михаил Сергеевич','kozlov','kozlov789','Преподаватель','kozlov@college.ru','89003334455'),(22,'Волкова Анна Павловна','volkova','volkova321','Преподаватель','volkova@college.ru','89004445566'),(24,'Петров Алексей Иванович','petrov_ai','SecurePass1!','Преподаватель','petrov@edu.ru','89123456789'),(25,'Сидорова Ольга Петровна','sidorova_op','Teacher2026#','Преподаватель','sidorova@edu.ru','89123456790'),(26,'Козлов Дмитрий Сергеевич','kozlov_ds','Kozlov#777','Преподаватель','kozlov@edu.ru','89123456791');
/*!40000 ALTER TABLE `teachers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-14  3:09:57
