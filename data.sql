USE `assets`;

INSERT INTO `regions` (`id`, `name`) VALUES
(8, 'Georgia'),
(5, 'Connecticut'),
(15, 'Kentucky'),
(1, 'Alabama'),
(4, 'Colorado'),
(9, 'Hawaii'),
(6, 'Delaware'),
(3, 'California'),
(17, 'Massachusetts'),
(11, 'Illinois'),
(12, 'Indiana'),
(2, 'Arizona'),
(13, 'Iowa'),
(7, 'Florida'),
(18, 'Nebraska'),
(10, 'Idaho'),
(14, 'Kansas'),
(16, 'Louisiana');

INSERT INTO `locations` (`id`, `name`, `regionId`) VALUES
(1, 'Montgomery', 1),
(2, 'Phoenix', 2),
(3, 'Sacramento', 3),
(4, 'Denver', 4),
(5, 'Hartford', 5),
(6, 'Dover', 6),
(7, 'Tallahassee', 7),
(8, 'Atlanta', 8),
(9, 'Honolulu', 9),
(10, 'Boise', 10),
(11, 'Tucson', 2),
(12, 'Springfield', 11),
(13, 'Indianapolis', 12),
(14, 'Wilmington', 6),
(15, 'Topeka', 14),
(16, 'Frankfort', 15),
(17, 'Des Moines', 13),
(18, 'Boston', 17),
(19, 'Baton Rouge', 16),
(20, 'New Castle', 6),
(22, 'Lincoln', 18),
(24, 'Boston', 17);

INSERT INTO `users` (`id`, `username`, `password`, `locationId`, `permission`, `active`) VALUES
(1, 'admin', 'dc647eb65e6711e155375218212b3964', 2, 9, 1),
(2, 'Username', 'dc647eb65e6711e155375218212b3964', 1, 9, 1),
(3, 'user0', '2d02e669731cbade6a64b58d602cf2a4', 6, 3, 0),
(4, 'user1', 'dc647eb65e6711e155375218212b3964', 8, 0, 1),
(5, 'user3', '594f803b380a41396ed63dca39503542', 3, 3, 1),
(6, 'user6', 'dc647eb65e6711e155375218212b3964', 5, 6, 1),
(7, 'Cindy Lauper', '2070e4cfb8f24209647d3c9ec55098ee', 9, 0, 1),
(8, 'Heather Nova', 'fd09276fe52af9e4812c157fa2bcd245', 10, 0, 1),
(9, 'George Michael', '2070e4cfb8f24209647d3c9ec55098ee', 8, 8, 1),
(10, 'Kylie Minogue', '2070e4cfb8f24209647d3c9ec55098ee', 19, 8, 1),
(11, 'John Lennon', '6b3d90e3b7eb6d50dc1f67da7837550d', 1, 4, 1),
(12, 'Ariana Grande', '594f803b380a41396ed63dca39503542', 4, 6, 1),
(13, 'Paul McCartney', 'c36af63f87acebba1c23498809db7537', 7, 1, 1),
(14, 'Lana Del Rey', '67c762276bced09ee4df0ed537d164ea', 12, 7, 1),
(15, 'Elvis Presley', '2d02e669731cbade6a64b58d602cf2a4', 13, 2, 1),
(16, 'Ringo Starr', '594f803b380a41396ed63dca39503542', 20, 8, 1),
(17, 'Bebe Rexha', '144c527ee6827ac129a794890507c712', 15, 8, 1),
(18, 'George Harrison', 'c36af63f87acebba1c23498809db7537', 3, 6, 1),
(19, 'Midge Ure', '97074bfab89d51b4d84cf9dc5974edc9', 2, 4, 0),
(20, 'Gary Numan', 'ee2327bcdf111b3cf2bd1bc7c126edf7', 15, 5, 1),
(21, 'Celine Dion', 'aadd9651472f9f14a783091027a4cec1', 5, 1, 1),
(22, 'Mich Jagger', '594f803b380a41396ed63dca39503542', 4, 3, 1),
(23, 'Ray Charles', '594f803b380a41396ed63dca39503542', 9, 2, 0),
(24, 'Karen Carpenter', '594f803b380a41396ed63dca39503542', 22, 1, 1),
(25, 'Freddy Mercury', 'ff49911a9e876e0d488cddc4177b85db', 11, 8, 1),
(26, 'Lene Lovich', 'd9853e2667fd6b121a4d560de44dc669', 12, 2, 1),
(36, 'Annabel Lamb', '594f803b380a41396ed63dca39503542', 6, 1, 1),
(37, 'Michael Jackson', 'fba2fdaf36fdf1931d552535a57eb984', 14, 1, 1),
(38, 'Elton John', 'a263982d8ae350fcee9fe612d94321a7', 16, 1, 1),
(39, 'Rita Ora', '594f803b380a41396ed63dca39503542', 17, 1, 1),
(40, 'Madonna', '594f803b380a41396ed63dca39503542', 18, 1, 1),
(42, 'Toyah Wilcox', '594f803b380a41396ed63dca39503542', 22, 1, 1);

INSERT INTO `products` (`id`, `name`, `buyUnitPrice`, `sellUnitPrice`) VALUES
(1, 'Product1', 580, 660),
(2, 'Product2', 2200, 2500),
(3, 'Product3', 1000, 1200),
(4, 'Product4', 120, 140),
(5, 'Product5', 115, 125),
(6, 'Product6', 5800, 6200),
(7, 'Product7', 18000, 19000),
(8, 'Product8', 45454, 45000),
(9, 'Product9', 150, 200),
(10, 'Product10', 500, 600),
(11, 'Product11', 150, 180),
(12, 'Product12', 60, 70),
(13, 'Product13', 656, 850),
(14, 'Plush dog', 850, 950),
(15, 'Product15', 5450, 5450),
(16, 'Product16', 5000, 5200),
(17, 'Product17', 500, 550),
(18, 'Product18', 100, 150),
(19, 'Product19', 450, 500),
(20, 'Product20', 120, 140),
(21, 'Product21', 150, 200),
(30, 'Plush cat', 500, 600),
(31, 'Product22', 400, 450);

INSERT INTO `stocks` (`id`, `productId`, `quantity`, `locationId`) VALUES
(2, 1, 0, 1),
(3, 2, 0, 1),
(4, 3, 0, 1),
(5, 4, 1, 2),
(6, 5, 0, 2),
(7, 1, 1, 2),
(8, 1, 0, 3),
(11, 1, 0, 11),
(12, 2, 0, 7),
(16, 2, 0, 2),
(17, 5, -23, 1),
(21, 3, 4, 2),
(23, 4, 1, 1);

INSERT INTO `purchases` (`id`, `productId`, `quantity`, `totalPrice`, `date`, `locationId`, `userId`) VALUES
(1, 1, 1, 1, '2022-02-10 01:03:00', 2, 1),
(2, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(3, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(4, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(5, 1, 1, 0, '2022-01-13 01:03:00', 2, 1),
(6, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(7, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(8, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(9, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(10, 1, 1, 0, '2022-02-13 01:03:00', 2, 1),
(11, 2, 3, 0, '2022-02-13 12:20:00', 1, 2),
(12, 2, 2, 0, '2022-02-13 12:20:00', 1, 2),
(13, 2, 2, 1, '2022-02-13 12:21:00', 1, 2),
(14, 2, 2, 1, '2022-02-13 12:20:00', 2, 2),
(15, 2, 2, 1, '2022-02-13 12:20:00', 1, 2),
(16, 2, 2, 1, '2022-02-13 12:20:00', 1, 2),
(17, 2, 2, 0, '2022-02-13 12:20:00', 1, 2),
(18, 2, 2, 0, '2022-02-13 12:20:00', 1, 2),
(19, 2, 2, 0, '2022-02-13 12:20:00', 1, 2),
(20, 2, 2, 18, '2022-02-15 12:20:00', 1, 2),
(21, 3, 1, 0, '2022-02-15 21:49:00', 1, 2),
(22, 3, 2, 0, '2022-02-15 21:49:00', 1, 2),
(23, 3, 2, 0, '2022-02-18 21:50:00', 1, 2),
(24, 4, 4, 0, '2022-02-18 21:55:00', 1, 2),
(27, 4, 4, 0, '2022-02-17 21:54:00', 2, 2),
(29, 4, 4, 0, '2022-02-17 21:54:00', 2, 2),
(30, 4, 4, 0, '2022-02-17 21:54:00', 2, 2),
(31, 4, 1, 1, '2022-02-20 13:14:00', 2, 1),
(32, 5, 1, 115, '2022-02-20 23:35:00', 1, 2),
(33, 5, 1, 115, '2022-02-20 23:35:00', 1, 2),
(34, 5, 2, 230, '2022-02-20 23:35:00', 1, 2),
(35, 5, 2, 230, '2022-02-20 23:35:00', 1, 2),
(36, 5, 2, 230, '2022-02-20 23:35:00', 1, 2),
(37, 5, 2, 230, '2022-02-21 00:13:00', 1, 2),
(38, 5, 1, 115, '2022-02-21 00:16:00', 1, 2),
(39, 5, 1, 115, '2022-02-21 00:16:00', 1, 2),
(40, 5, 2, 230, '2022-02-21 00:16:00', 1, 2),
(41, 5, 2, 230, '2022-02-21 00:16:00', 1, 2),
(42, 4, 1, 1, '2022-02-20 13:14:00', 2, 2),
(43, 5, 1, 115, '2022-02-20 23:35:00', 1, 2),
(44, 5, 1, 115, '2022-02-20 23:35:00', 1, 2),
(45, 5, 2, 230, '2022-02-20 23:35:00', 1, 2),
(46, 5, 3, 345, '2022-02-20 23:35:00', 1, 2),
(47, 5, 2, 230, '2022-02-20 23:35:00', 1, 2),
(48, 5, 2, 230, '2022-02-21 00:13:00', 1, 2),
(49, 5, 1, 115, '2022-02-21 00:16:00', 1, 2),
(50, 5, 1, 115, '2022-02-21 00:16:00', 1, 2),
(51, 5, 2, 230, '2022-02-21 00:16:00', 1, 2),
(52, 5, 2, 230, '2022-02-21 00:16:00', 1, 2),
(53, 5, 1, 115, '2022-02-25 22:34:00', 1, 2),
(54, 5, 1, 115, '2022-02-25 23:07:00', 1, 2),
(55, 5, 1, 115, '2022-02-25 23:08:00', 1, 2),
(56, 5, 1, 115, '2022-02-25 23:08:00', 1, 2),
(57, 5, 1, 115, '2022-02-25 23:08:00', 1, 2),
(58, 5, 1, 115, '2022-02-25 23:08:00', 1, 2),
(59, 5, 1, 115, '2022-02-25 23:08:00', 1, 2),
(60, 5, 1, 115, '2022-02-25 23:15:00', 1, 2),
(61, 5, 1, 115, '2022-02-25 23:15:00', 1, 2),
(62, 5, 1, 115, '2022-02-25 23:15:00', 1, 2),
(63, 5, 1, 115, '2022-02-25 23:15:00', 1, 2),
(64, 5, 1, 115, '2022-02-25 23:17:00', 1, 2),
(65, 5, 1, 115, '2022-02-25 23:17:00', 1, 2),
(66, 5, 1, 115, '2022-02-25 23:17:00', 1, 2),
(67, 5, 1, 115, '2022-02-25 23:17:00', 1, 2),
(68, 5, 1, 115, '2022-02-25 23:17:00', 1, 2),
(69, 5, 1, 115, '2022-02-25 23:19:00', 1, 2),
(70, 5, 1, 115, '2022-02-25 23:19:00', 1, 2),
(71, 5, 1, 115, '2022-02-25 23:19:00', 1, 2),
(72, 5, 1, 115, '2022-02-25 23:19:00', 1, 2),
(73, 5, 1, 115, '2022-02-25 23:20:00', 1, 2),
(74, 5, 1, 115, '2022-02-25 23:20:00', 1, 2),
(75, 5, 1, 115, '2022-02-25 23:20:00', 1, 2),
(76, 5, 1, 115, '2022-02-25 23:20:00', 1, 2),
(77, 5, 1, 115, '2022-02-25 23:20:00', 1, 2),
(78, 5, 1, 115, '2022-02-25 23:21:00', 1, 2),
(79, 5, 1, 115, '2022-02-25 23:21:00', 1, 2),
(80, 5, 1, 115, '2022-02-25 23:21:00', 1, 2),
(81, 5, 1, 115, '2022-02-25 23:22:00', 1, 2),
(82, 5, 1, 115, '2022-02-25 23:22:00', 1, 2),
(83, 5, 1, 115, '2022-02-25 23:23:00', 1, 2),
(84, 5, 1, 115, '2022-02-25 23:23:00', 2, 2),
(85, 5, 1, 115, '2022-02-25 23:24:00', 1, 2),
(86, 5, 1, 115, '2022-02-25 23:24:00', 1, 2),
(89, 5, 1, 115, '2022-02-25 23:24:00', 1, 2),
(91, 5, 1, 115, '2022-02-25 23:28:00', 1, 2),
(93, 5, 1, 115, '2022-02-25 23:30:00', 1, 2),
(95, 5, 1, 115, '2022-02-25 23:31:00', 1, 2),
(100, 6, 1, 500, '2022-02-27 02:46:00', 1, 2),
(101, 6, 1, 500, '2022-02-27 02:46:00', 1, 2),
(102, 6, 1, 500, '2022-02-27 02:46:00', 1, 2),
(103, 6, 2, 11600, '2022-02-27 02:48:00', 2, 2),
(104, 6, 1, 500, '2022-02-27 02:46:00', 1, 2),
(105, 6, 1, 500, '2022-02-27 02:46:00', 1, 2),
(106, 6, 2, 11600, '2022-02-27 02:46:00', 1, 2),
(107, 6, 1, 5800, '2022-02-27 12:09:00', 1, 2),
(108, 6, 1, 5800, '2022-02-27 12:10:00', 1, 2),
(109, 6, 1, 5800, '2022-02-27 12:13:00', 1, 2),
(110, 3, 30, 30000, '2022-03-20 17:46:17', 1, 2),
(111, 7, 20, 360000, '2022-03-25 21:09:00', 1, 2),
(112, 6, 20, 116000, '2022-03-25 21:10:00', 1, 2),
(113, 4, 1, 120, '2022-04-02 18:04:00', 1, 2),
(114, 3, 1, 1000, '2022-04-04 01:17:00', 2, 2),
(115, 3, 1, 1000, '2022-04-04 01:20:00', 2, 2),
(116, 3, 1, 1000, '2022-04-04 01:22:00', 2, 2),
(117, 3, 1, 1000, '2022-04-04 01:23:00', 2, 2),
(118, 3, 1, 1000, '2022-04-04 01:51:00', 2, 2),
(119, 3, 1, 1000, '2022-04-04 02:01:00', 2, 2),
(120, 4, 1, 120, '2022-04-04 02:02:00', 2, 2),
(121, 4, 1, 120, '2022-04-04 02:03:00', 1, 2),
(122, 4, 1, 120, '2022-04-04 02:04:00', 1, 2),
(123, 3, 1, 1000, '2022-04-04 02:10:00', 2, 2),
(124, 4, 1, 120, '2022-04-04 02:11:00', 1, 2),
(125, 4, 1, 120, '2022-04-04 02:23:00', 1, 2),
(126, 4, 1, 120, '2022-04-04 02:25:00', 1, 2),
(127, 4, 1, 120, '2022-04-04 02:27:00', 1, 2),
(128, 4, 1, 120, '2022-04-04 02:28:00', 1, 2);

INSERT INTO `sales` (`id`, `productId`, `quantity`, `totalPrice`, `date`, `locationId`, `userId`) VALUES
(1, 3, 2, 2000, '2022-02-21 00:18:00', 10, 8),
(2, 3, 10, 10000, '2022-02-21 00:18:00', 20, 16),
(3, 1, 4, 160, '2022-02-20 23:55:00', 20, 16),
(4, 2, 2, 4400, '2022-02-21 00:18:00', 19, 10),
(5, 3, 4, 400, '2022-02-21 00:18:00', 1, 21),
(6, 3, 4, 400, '2022-02-21 00:18:00', 20, 16),
(7, 3, 4, 400, '2022-02-21 00:18:00', 1, 21),
(8, 4, 3, 340, '2022-02-21 00:52:00', 12, 14),
(9, 4, 5, 600, '2022-02-21 00:52:00', 20, 16),
(10, 4, 10, 1200, '2022-02-27 12:07:00', 12, 14),
(11, 5, 3, 345, '2022-02-27 12:08:00', 13, 15),
(12, 4, 10, 1200, '2022-02-27 12:08:00', 17, 19),
(13, 4, 4, 480, '2022-02-27 12:14:00', 15, 20),
(14, 4, 2, 240, '2022-02-27 12:14:00', 16, 18),
(15, 4, 15, 1800, '2022-03-12 17:39:00', 20, 16),
(16, 4, 12, 1440, '2022-03-12 17:39:00', 1, 21),
(17, 1, 1, 570, '2022-02-12 23:39:00', 1, 21),
(18, 1, 1, 570, '2022-02-12 23:39:00', 19, 10),
(19, 1, 1, 570, '2022-02-12 23:39:00', 19, 10),
(20, 1, 1, 570, '2022-02-12 23:39:00', 19, 10),
(21, 1, 1, 570, '2022-02-12 23:39:00', 12, 14),
(22, 1, 1, 570, '2022-02-12 23:39:00', 12, 14),
(23, 1, 1, 0, '2022-02-12 23:39:00', 1, 2),
(24, 3, 2, 2000, '2022-02-21 00:18:00', 10, 2),
(25, 3, 10, 10000, '2022-02-21 00:18:00', 20, 2),
(26, 1, 4, 160, '2022-02-20 23:55:00', 20, 2),
(27, 2, 2, 4400, '2022-02-21 00:18:00', 19, 2),
(28, 3, 4, 400, '2022-02-21 00:18:00', 1, 2),
(29, 3, 4, 400, '2022-02-21 00:18:00', 20, 2),
(30, 3, 4, 400, '2022-02-21 00:18:00', 1, 2),
(31, 4, 3, 340, '2022-02-21 00:52:00', 12, 2),
(32, 4, 5, 600, '2022-02-21 00:52:00', 20, 2),
(33, 4, 10, 1200, '2022-02-27 12:07:00', 12, 2),
(34, 5, 3, 345, '2022-02-27 12:08:00', 13, 2),
(35, 4, 10, 1200, '2022-02-27 12:08:00', 17, 2),
(36, 4, 4, 480, '2022-02-27 12:14:00', 15, 2),
(37, 4, 2, 240, '2022-02-27 12:14:00', 16, 2),
(38, 4, 15, 1800, '2022-03-12 17:39:00', 9, 7),
(39, 4, 12, 1440, '2022-03-12 17:39:00', 10, 8),
(40, 2, 2, 4400, '2022-02-21 00:18:00', 19, 2),
(41, 3, 4, 400, '2022-02-21 00:18:00', 1, 2),
(42, 3, 5, 400, '2022-02-21 00:18:00', 20, 2),
(43, 3, 4, 400, '2022-02-21 00:18:00', 1, 2),
(44, 4, 3, 340, '2022-02-21 00:52:00', 12, 2),
(45, 4, 5, 600, '2022-02-21 00:52:00', 20, 2),
(46, 4, 10, 1200, '2022-02-27 12:07:00', 12, 2),
(47, 5, 3, 345, '2022-02-27 12:08:00', 13, 2),
(48, 4, 10, 1200, '2022-02-27 12:08:00', 17, 2),
(49, 4, 4, 480, '2022-02-27 12:14:00', 15, 2),
(50, 4, 4, 480, '2022-02-27 12:14:00', 16, 2),
(52, 4, 12, 1550, '2022-03-12 17:38:00', 19, 10),
(53, 1, 4, 160, '2022-02-20 23:55:00', 20, 2),
(555, 3, 10, 12000, '2022-03-20 17:31:23', 19, 10),
(556, 4, 50, 6000, '2022-03-20 17:33:58', 20, 16),
(557, 3, 31, 37200, '2022-03-20 17:47:28', 12, 14),
(558, 4, 41, 5740, '2022-03-20 17:54:38', 9, 7),
(559, 5, 6, 690, '2022-03-25 21:12:00', 1, 2),
(560, 4, 4, 480, '2022-03-26 13:06:34', 12, 14),
(561, 5, 4, 1000, '2022-03-26 13:06:00', 1, 21),
(563, 5, 4, 1000, '2022-03-26 13:06:00', 1, 21),
(564, 5, 4, 1000, '2022-03-26 13:07:00', 1, 21),
(565, 5, 1, 115, '2022-03-26 13:08:00', 1, 2),
(566, 5, 1, 115, '2022-03-26 13:08:00', 1, 2),
(567, 4, 1, 120, '2022-04-04 02:31:00', 1, 2),
(568, 4, 1, 120, '2022-04-04 02:34:00', 1, 2);