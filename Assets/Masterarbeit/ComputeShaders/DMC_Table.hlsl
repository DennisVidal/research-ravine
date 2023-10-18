#ifndef DUAL_MARCHING_CUBES_TABLE
#define DUAL_MARCHING_CUBES_TABLE

const static uint2 DMC_EDGE_TABLE_OFFSETS[256] =
{
        uint2(0, 0),       uint2(0, 3),       uint2(3, 6),       uint2(6, 10),      uint2(10, 13),     uint2(13, 20),     uint2(20, 24),     uint2(24, 29),
        uint2(29, 32),     uint2(32, 36),     uint2(36, 43),     uint2(43, 48),     uint2(48, 52),     uint2(52, 57),     uint2(57, 62),     uint2(62, 66),
        uint2(66, 69),     uint2(69, 73),     uint2(73, 80),     uint2(80, 85),     uint2(85, 92),     uint2(92, 100),    uint2(100, 108),   uint2(108, 114),
        uint2(114, 121),   uint2(121, 126),   uint2(126, 137),   uint2(137, 143),   uint2(143, 151),   uint2(151, 157),   uint2(157, 166),   uint2(166, 171),
        uint2(171, 174),   uint2(174, 181),   uint2(181, 185),   uint2(185, 190),   uint2(190, 197),   uint2(197, 208),   uint2(208, 213),   uint2(213, 219),
        uint2(219, 226),   uint2(226, 234),   uint2(234, 242),   uint2(242, 248),   uint2(248, 256),   uint2(256, 265),   uint2(265, 271),   uint2(271, 276),
        uint2(276, 280),   uint2(280, 285),   uint2(285, 290),   uint2(290, 294),   uint2(294, 302),   uint2(302, 311),   uint2(311, 317),   uint2(317, 322),
        uint2(322, 330),   uint2(330, 336),   uint2(336, 345),   uint2(345, 350),   uint2(350, 359),   uint2(359, 366),   uint2(366, 373),   uint2(373, 377),
        uint2(377, 380),   uint2(380, 387),   uint2(387, 394),   uint2(394, 402),   uint2(402, 406),   uint2(406, 414),   uint2(414, 419),   uint2(419, 425),
        uint2(425, 432),   uint2(432, 440),   uint2(440, 451),   uint2(451, 460),   uint2(460, 465),   uint2(465, 471),   uint2(471, 477),   uint2(477, 482),
        uint2(482, 489),   uint2(489, 497),   uint2(497, 508),   uint2(508, 517),   uint2(517, 525),   uint2(525, 534),   uint2(534, 543),   uint2(543, 550),
        uint2(550, 561),   uint2(561, 570),   uint2(570, 585),   uint2(585, 595),   uint2(595, 604),   uint2(604, 611),   uint2(611, 621),   uint2(621, 627),
        uint2(627, 631),   uint2(631, 639),   uint2(639, 644),   uint2(644, 650),   uint2(650, 655),   uint2(655, 664),   uint2(664, 668),   uint2(668, 673),
        uint2(673, 681),   uint2(681, 690),   uint2(690, 699),   uint2(699, 706),   uint2(706, 712),   uint2(712, 719),   uint2(719, 724),   uint2(724, 728),
        uint2(728, 733),   uint2(733, 739),   uint2(739, 745),   uint2(745, 750),   uint2(750, 756),   uint2(756, 763),   uint2(763, 768),   uint2(768, 772),
        uint2(772, 781),   uint2(781, 788),   uint2(788, 798),   uint2(798, 804),   uint2(804, 811),   uint2(811, 817),   uint2(817, 823),   uint2(823, 826),
        uint2(826, 829),   uint2(829, 836),   uint2(836, 843),   uint2(843, 851),   uint2(851, 858),   uint2(858, 869),   uint2(869, 877),   uint2(877, 886),
        uint2(886, 890),   uint2(890, 895),   uint2(895, 903),   uint2(903, 909),   uint2(909, 914),   uint2(914, 920),   uint2(920, 926),   uint2(926, 931),
        uint2(931, 935),   uint2(935, 940),   uint2(940, 948),   uint2(948, 954),   uint2(954, 962),   uint2(962, 971),   uint2(971, 980),   uint2(980, 987),
        uint2(987, 992),   uint2(992, 996),   uint2(996, 1005),  uint2(1005, 1010), uint2(1010, 1016), uint2(1016, 1021), uint2(1021, 1028), uint2(1028, 1032),
        uint2(1032, 1039), uint2(1039, 1050), uint2(1050, 1058), uint2(1058, 1067), uint2(1067, 1078), uint2(1078, 1093), uint2(1093, 1102), uint2(1102, 1112),
        uint2(1112, 1120), uint2(1120, 1129), uint2(1129, 1138), uint2(1138, 1145), uint2(1145, 1154), uint2(1154, 1164), uint2(1164, 1171), uint2(1171, 1177),
        uint2(1177, 1182), uint2(1182, 1188), uint2(1188, 1194), uint2(1194, 1199), uint2(1199, 1208), uint2(1208, 1218), uint2(1218, 1225), uint2(1225, 1231),
        uint2(1231, 1237), uint2(1237, 1242), uint2(1242, 1249), uint2(1249, 1253), uint2(1253, 1260), uint2(1260, 1266), uint2(1266, 1272), uint2(1272, 1275),
        uint2(1275, 1279), uint2(1279, 1287), uint2(1287, 1295), uint2(1295, 1304), uint2(1304, 1309), uint2(1309, 1318), uint2(1318, 1324), uint2(1324, 1331),
        uint2(1331, 1336), uint2(1336, 1342), uint2(1342, 1351), uint2(1351, 1358), uint2(1358, 1362), uint2(1362, 1367), uint2(1367, 1372), uint2(1372, 1376),
        uint2(1376, 1381), uint2(1381, 1387), uint2(1387, 1396), uint2(1396, 1403), uint2(1403, 1409), uint2(1409, 1416), uint2(1416, 1423), uint2(1423, 1429),
        uint2(1429, 1435), uint2(1435, 1440), uint2(1440, 1450), uint2(1450, 1456), uint2(1456, 1461), uint2(1461, 1465), uint2(1465, 1471), uint2(1471, 1474),
        uint2(1474, 1479), uint2(1479, 1488), uint2(1488, 1494), uint2(1494, 1501), uint2(1501, 1507), uint2(1507, 1517), uint2(1517, 1522), uint2(1522, 1528),
        uint2(1528, 1534), uint2(1534, 1541), uint2(1541, 1548), uint2(1548, 1554), uint2(1554, 1559), uint2(1559, 1565), uint2(1565, 1569), uint2(1569, 1572),
        uint2(1572, 1576), uint2(1576, 1581), uint2(1581, 1586), uint2(1586, 1590), uint2(1590, 1595), uint2(1595, 1601), uint2(1601, 1605), uint2(1605, 1608),
        uint2(1608, 1613), uint2(1613, 1617), uint2(1617, 1623), uint2(1623, 1626), uint2(1626, 1630), uint2(1630, 1633), uint2(1633, 1636), uint2(1636, 1636)
    };

const static uint DMC_EDGE_TABLE[1636] =
{
    0, 8, 3,
    0, 1, 9,
    1, 3, 8, 9,
    1, 2, 10,
    0, 8, 3, 12, 1, 2, 10,
    9, 0, 2, 10,
    10, 2, 3, 8, 9,

    3, 11, 2,
    0, 8, 11, 2,
    1, 9, 0, 12, 2, 3, 11,
    1, 2, 8, 9, 11,
    1, 3, 10, 11,
    0, 1, 8, 10, 11,
    0, 3, 9, 10, 11,
    8, 9, 10, 11,

    4, 7, 8,
    0, 3, 4, 7,
    0, 1, 9, 12, 8, 4, 7,
    1, 3, 4, 7, 9,
    1, 2, 10, 12, 8, 4, 7,
    1, 2, 10, 12, 0, 3, 7, 4,
    0, 2, 10, 9, 12, 8, 7, 4,
    2, 3, 4, 7, 9, 10,

    8, 4, 7, 12, 3, 11, 2,
    0, 2, 4, 7, 11,
    9, 0, 1, 12, 8, 4, 7, 12, 2, 3, 11,
    1, 2, 4, 7, 9, 11,
    3, 11, 10, 1, 12, 8, 7, 4,
    0, 1, 4, 7, 10, 11,
    4, 7, 8, 12, 0, 3, 11, 10, 9,
    4, 7, 9, 10, 11,

    9, 5, 4,
    9, 5, 4, 12, 0, 8, 3,
    0, 5, 4, 1,
    1, 3, 4, 5, 8,
    1, 2, 10, 12, 9, 5, 4,
    3, 0, 8, 12, 1, 2, 10, 12, 4, 9, 5,
    0, 2, 4, 5, 10,
    2, 3, 4, 5, 8, 10,

    9, 5, 4, 12, 2, 3, 11,
    9, 4, 5, 12, 0, 2, 11, 8,
    3, 11, 2, 12, 0, 4, 5, 1,
    1, 2, 4, 5, 8, 11,
    3, 11, 10, 1, 12, 9, 4, 5,
    4, 9, 5, 12, 0, 1, 8, 10, 11,
    0, 3, 4, 5, 10, 11,
    5, 4, 8, 10, 11,

    5, 7, 8, 9,
    0, 3, 5, 7, 9,
    0, 1, 5, 7, 8,
    1, 5, 3, 7,
    1, 2, 10, 12, 8, 7, 5, 9,
    1, 2, 10, 12, 0, 3, 7, 5, 9,
    0, 2, 5, 7, 8, 10,
    2, 3, 5, 7, 10,

    2, 3, 11, 12, 5, 7, 8, 9,
    0, 2, 5, 7, 9, 11,
    2, 3, 11, 12, 0, 1, 5, 7, 8,
    1, 2, 5, 7, 11,
    3, 11, 10, 1, 12, 8, 7, 5, 9,
    0, 1, 5, 7, 9, 10, 11,
    0, 3, 5, 7, 8, 10, 11,
    5, 7, 10, 11,

    5, 6, 10,
    0, 3, 8, 12, 10, 5, 6,
    0, 1, 9, 12, 10, 5, 6,
    10, 5, 6, 12, 3, 8, 9, 1,
    1, 2, 5, 6,
    0, 3, 8, 12, 1, 2, 6, 5,
    0, 2, 5, 6, 9,
    2, 3, 5, 6, 8, 9,

    3, 11, 2, 12, 10, 6, 5,
    10, 5, 6, 12, 0, 8, 2, 11,
    3, 11, 2, 12, 0, 1, 9, 12, 10, 5, 6,
    10, 5, 6, 12, 11, 2, 8, 9, 1,
    1, 3, 5, 6, 11,
    0, 1, 5, 6, 8, 11,
    0, 3, 5, 6, 9, 11,
    5, 6, 8, 9, 11,

    8, 7, 4, 12, 5, 6, 10,
    5, 6, 10, 12, 0, 3, 4, 7,
    10, 5, 6, 12, 1, 9, 0, 12, 8, 7, 4,
    10, 5, 6, 12, 7, 4, 9, 1, 3,
    8, 7, 4, 12, 1, 2, 6, 5,
    0, 3, 7, 4, 12, 1, 2, 6, 5,
    8, 7, 4, 12, 0, 9, 2, 5, 6,
    2, 3, 4, 5, 6, 7, 9,

    3, 11, 2, 12, 5, 6, 10, 12, 8, 7, 4,
    10, 5, 6, 12, 0, 2, 11, 7, 4,
    3, 11, 2, 12, 0, 1, 9, 12, 10, 5, 6, 12, 8, 7, 4,
    10, 5, 6, 12, 7, 4, 11, 2, 1, 9,
    8, 7, 4, 12, 3, 11, 6, 5, 1,
    0, 1, 4, 5, 6, 7, 11,
    8, 7, 4, 12, 6, 5, 9, 0, 11, 3,
    4, 5, 6, 7, 9, 11,

    4, 6, 9, 10,
    0, 3, 8, 12, 9, 10, 6, 4,
    0, 1, 4, 6, 10,
    1, 3, 4, 6, 8, 10,
    1, 2, 4, 6, 9,
    0, 3, 8, 12, 1, 2, 4, 6, 9,
    0, 2, 4, 6,
    2, 3, 4, 6, 8,

    11, 2, 3, 12, 9, 4, 10, 6,
    0, 2, 11, 8, 12, 9, 4, 6, 10,
    2, 3, 11, 12, 0, 1, 4, 6, 10,
    1, 2, 4, 6, 8, 10, 11,
    1, 3, 4, 6, 9, 11,
    0, 1, 4, 6, 8, 9, 11,
    0, 3, 4, 6, 11,
    4, 6, 8, 11,

    6, 7, 8, 9, 10,
    0, 3, 6, 7, 9, 10,
    0, 1, 6, 7, 8, 10,
    1, 3, 6, 7, 10,
    1, 2, 6, 7, 8, 9,
    0, 1, 2, 3, 6, 7, 9,
    0, 2, 6, 7, 8,
    2, 3, 6, 7,

    3, 11, 2, 12, 10, 6, 9, 7, 8,
    0, 2, 6, 7, 9, 10, 11,
    3, 11, 2, 12, 8, 7, 0, 1, 10, 6,
    1, 2, 6, 7, 10, 11,
    1, 3, 6, 7, 8, 9, 11,
    0, 1, 6, 7, 9, 11,
    0, 3, 6, 7, 8, 11,
    6, 7, 11,

    11, 7, 6,
    11, 7, 6, 12, 0, 3, 8,
    11, 7, 6, 12, 0, 9, 1,
    11, 7, 6, 12, 1, 3, 8, 9,
    11, 7, 6, 12, 1, 2, 10,
    11, 7, 6, 12, 1, 2, 10, 12, 0, 3, 8,
    11, 7, 6, 12, 0, 9, 10, 2,
    11, 7, 6, 12, 2, 3, 8, 9, 10,

    2, 3, 6, 7,
    0, 2, 6, 7, 8,
    0, 1, 9, 12, 3, 2, 6, 7,
    1, 2, 6, 7, 8, 9,
    1, 3, 6, 7, 10,
    0, 1, 6, 7, 8, 10,
    0, 3, 6, 7, 9, 10,
    6, 7, 8, 9, 10,

    4, 6, 8, 11,
    0, 3, 4, 6, 11,
    0, 1, 9, 12, 8, 4, 11, 6,
    1, 3, 4, 6, 9, 11,
    1, 2, 10, 12, 8, 4, 6, 11,
    1, 2, 10, 12, 0, 3, 11, 6, 4,
    0, 9, 10, 2, 12, 8, 4, 11, 6,
    2, 3, 4, 6, 9, 10, 11,

    2, 3, 4, 6, 8,
    0, 2, 4, 6,
    0, 1, 9, 12, 2, 3, 8, 4, 6,
    1, 2, 4, 6, 9,
    1, 3, 4, 6, 8, 10,
    0, 1, 4, 6, 10,
    0, 3, 4, 6, 8, 9, 10,
    4, 6, 9, 10,

    6, 7, 11, 12, 4, 5, 9,
    6, 7, 11, 12, 4, 5, 9, 12, 0, 3, 8,
    11, 7, 6, 12, 1, 0, 5, 4,
    11, 7, 6, 12, 8, 3, 1, 5, 4,
    11, 7, 6, 12, 4, 5, 9, 12, 1, 2, 10,
    11, 7, 6, 12, 4, 5, 9, 12, 1, 2, 10, 12, 0, 3, 8,
    11, 7, 6, 12, 0, 2, 10, 5, 4,
    11, 7, 6, 12, 8, 3, 2, 10, 5, 4,

    4, 5, 9, 12, 3, 2, 6, 7,
    4, 5, 9, 12, 2, 0, 8, 7, 6,
    3, 2, 6, 7, 12, 0, 1, 5, 4,
    1, 2, 4, 5, 6, 7, 8,
    9, 4, 5, 12, 1, 10, 6, 7, 3,
    9, 4, 5, 12, 6, 10, 1, 0, 8, 7,
    0, 3, 4, 5, 6, 7, 10,
    4, 5, 6, 7, 8, 10,

    5, 6, 8, 9, 11,
    0, 3, 5, 6, 9, 11,
    0, 1, 5, 6, 8, 11,
    1, 3, 5, 6, 11,
    1, 2, 10, 12, 9, 5, 6, 11, 8,
    1, 2, 10, 12, 9, 0, 3, 11, 6, 5,
    0, 2, 5, 6, 8, 10, 11,
    2, 3, 5, 6, 10, 11,

    2, 3, 5, 6, 8, 9,
    0, 2, 5, 6, 9,
    0, 1, 2, 3, 5, 6, 8,
    1, 2, 5, 6,
    1, 3, 5, 6, 8, 9, 10,
    0, 1, 5, 6, 9, 10,
    0, 3, 5, 6, 8, 10,
    5, 6, 10,

    5, 7, 10, 11,
    5, 7, 10, 11, 12, 0, 3, 8,
    5, 7, 10, 11, 12, 0, 1, 9,
    5, 7, 10, 11, 12, 3, 8, 9, 1,
    1, 2, 5, 7, 11,
    1, 2, 5, 7, 11, 12, 0, 3, 8,
    0, 2, 5, 7, 9, 11,
    2, 3, 5, 7, 8, 9, 11,

    2, 3, 5, 7, 10,
    0, 2, 5, 7, 8, 10,
    0, 1, 9, 12, 7, 3, 2, 10, 5,
    1, 2, 5, 7, 8, 9, 10,
    1, 3, 5, 7,
    0, 1, 5, 7, 8,
    0, 3, 5, 7, 9,
    5, 7, 8, 9,

    4, 5, 8, 10, 11,
    0, 3, 4, 5, 10, 11,
    0, 1, 9, 12, 8, 11, 10, 4, 5,
    1, 3, 4, 5, 9, 10, 11,
    1, 2, 4, 5, 8, 11,
    0, 1, 2, 3, 4, 5, 11,
    0, 2, 4, 5, 8, 9, 11,
    2, 3, 4, 5, 9, 11,

    2, 3, 4, 5, 8, 10,
    0, 2, 4, 5, 10,
    0, 1, 9, 12, 8, 3, 2, 10, 5, 4,
    1, 2, 4, 5, 9, 10,
    1, 3, 4, 5, 8,
    0, 1, 4, 5,
    0, 3, 4, 5, 8, 9,
    4, 5, 9,

    4, 7, 9, 10, 11,
    0, 3, 8, 12, 10, 9, 4, 7, 11,
    0, 1, 4, 7, 10, 11,
    1, 3, 4, 7, 8, 10, 11,
    1, 2, 4, 7, 9, 11,
    1, 2, 4, 7, 9, 11, 12, 0, 3, 8,
    0, 2, 4, 7, 11,
    2, 3, 4, 7, 8, 11,

    2, 3, 4, 7, 9, 10,
    0, 2, 4, 7, 8, 9, 10,
    0, 1, 2, 3, 4, 7, 10,
    1, 2, 4, 7, 8, 10,
    1, 3, 4, 7, 9,
    0, 1, 4, 7, 8, 9,
    0, 3, 4, 7,
    4, 7, 8,

    8, 9, 10, 11,
    0, 3, 9, 10, 11,
    0, 1, 8, 10, 11,
    1, 3, 10, 11,
    1, 2, 8, 9, 11,
    0, 1, 2, 3, 9, 11,
    0, 2, 8, 11,
    2, 3, 11,

    2, 3, 8, 9, 10,
    0, 2, 9, 10,
    0, 1, 2, 3, 8, 10,
    1, 2, 10,
    1, 3, 8, 9,
    0, 1, 9,
    0, 3, 8
};

const static uint2 DMC_EDGE_CORNERS[12] =
{
    uint2(0, 1), uint2(1, 2), uint2(3, 2), uint2(0, 3),
    uint2(4, 5), uint2(5, 6), uint2(7, 6), uint2(4, 7),
    uint2(0, 4), uint2(1, 5), uint2(2, 6), uint2(3, 7)
};

const static float3 DMC_CORNER_OFFSETS[8] =
{
    float3(0, 0, 0), float3(0, 0, 1), float3(1, 0, 1), float3(1, 0, 0),
    float3(0, 1, 0), float3(0, 1, 1), float3(1, 1, 1), float3(1, 1, 0)
};

const static uint3 DMC_EDGE_OFFSETS[12] =
{
    uint3(0, 0, 0), uint3(0, 0, 1), uint3(1, 0, 0), uint3(0, 0, 0),
    uint3(0, 1, 0), uint3(0, 1, 1), uint3(1, 1, 0), uint3(0, 1, 0),
    uint3(0, 0, 0), uint3(0, 0, 1), uint3(1, 0, 1), uint3(1, 0, 0)
};

const static uint2 DMC_EDGE_VERTEX_INDEX_MAPPING[12] =
{
    uint2(0, 3), uint2(1, 1), uint2(0, 2), uint2(1, 3),
    uint2(0, 1), uint2(1, 0), uint2(0, 0), uint2(1, 2),
    uint2(2, 3), uint2(2, 2), uint2(2, 0), uint2(2, 1)
    
    //uint2(0, 3), uint2(1, 1), uint2(0, 2), uint2(1, 3),
    //uint2(0, 1), uint2(1, 0), uint2(0, 0), uint2(1, 2),
    //uint2(2, 3), uint2(2, 1), uint2(2, 0), uint2(2, 2)
};

//const static int2x4 DMC_VERTEX_ORDER_TABLE[3] = {
//    int2x4(0, 1, 2, 3, 0, 2, 1, 3),
//    int2x4(0, 1, 2, 3, 0, 2, 1, 3),
//    int2x4(0, 2, 1, 3, 0, 1, 2, 3)
//};

const static int DMC_CELL_VERTEX_COUNTS[256] =
{
    0, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1,
    1, 1, 2, 1, 2, 2, 2, 1, 2, 1, 3, 1, 2, 1, 2, 1,
    1, 2, 1, 1, 2, 3, 1, 1, 2, 2, 2, 1, 2, 2, 1, 1,
    1, 1, 1, 1, 2, 2, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1,
    1, 2, 2, 2, 1, 2, 1, 1, 2, 2, 3, 2, 1, 1, 1, 1,
    2, 2, 3, 2, 2, 2, 2, 1, 3, 2, 4, 2, 2, 1, 2, 1,
    1, 2, 1, 1, 1, 2, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1,
    1, 2, 2, 2, 2, 3, 2, 2, 1, 1, 2, 1, 1, 1, 1, 1,
    1, 1, 2, 1, 2, 2, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1,
    2, 3, 2, 2, 3, 4, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1,
    1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 2, 2, 2, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1,
    1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1,
    1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
};
#endif