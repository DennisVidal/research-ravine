using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//Working DMC
/*
public class DualMarchingCubes
{
    #region Enums
    //     5----------6
    //    /|         /|
    //   / |        / |
    //  4----------7  |
    //  |  |       |  |
    //  |  1-------|--2     
    //  | /        | /      y z
    //  |/         |/       |/
    //  0----------3        +--X
    public enum CubeCorner
    {
        LEFT_BOTTOM_BACK,
        LEFT_BOTTOM_FRONT,
        RIGHT_BOTTOM_FRONT,
        RIGHT_BOTTOM_BACK,
        LEFT_TOP_BACK,
        LEFT_TOP_FRONT,
        RIGHT_TOP_FRONT,
        RIGHT_TOP_BACK,
    }



    //          
    //      #----5-----#
    //     /|         /|
    //    4 9        6 |
    //   /  |       /  10
    //  #----7-----#   |
    //  |   |      |   |
    //  8   #----1-|---#     
    //  |  /       11 /      
    //  | 0        | 2      y z 
    //  |/         |/       |/
    //  #----3-----#        +--X
    //       
    public enum CubeEdge
    {
        LEFT_BOTTOM,
        FRONT_BOTTOM,
        RIGHT_BOTTOM,
        BACK_BOTTOM,
        LEFT_TOP,
        FRONT_TOP,
        RIGHT_TOP,
        BACK_TOP,
        LEFT_BACK,
        LEFT_FRONT,
        RIGHT_FRONT,
        RIGHT_BACK
    }
    #endregion Enums
    #region Tables
    //Tables taken from https://stackoverflow.com/questions/16638711/dual-marching-cubes-table

    //Relevant edges for each vertex, separated by -1
    public static int[][][] edgeTable = new int[][][]
    {
            new int[][] { null },
            new int[][] {new int[]{0, 8, 3}},
            new int[][] {new int[]{0, 1, 9}},
            new int[][] {new int[]{1, 3, 8, 9}},
            new int[][] {new int[]{1, 2, 10}},
            new int[][] { new int[] { 0, 8, 3 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{9, 0, 2, 10}},
            new int[][] {new int[]{10, 2, 3, 8, 9}},
            new int[][] {new int[]{3, 11, 2}},
            new int[][] {new int[]{0, 8, 11, 2}},
            new int[][] {new int[]{1, 9, 0 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{1, 2, 8, 9, 11}},
            new int[][] {new int[]{1, 3, 10, 11}},
            new int[][] {new int[]{0, 1, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 9, 10, 11}},
            new int[][] {new int[]{8, 9, 10, 11}},
            new int[][] {new int[]{4, 7, 8}},
            new int[][] {new int[]{0, 3, 4, 7}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 4, 7}},
            new int[][] {new int[]{1, 3, 4, 7, 9}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 4, 7}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 7, 4}},
            new int[][] {new int[]{0, 2, 10, 9 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{2, 3, 4, 7, 9, 10}},
            new int[][] {new int[]{8, 4, 7 }, new int[] { 3, 11, 2}},
            new int[][] {new int[]{0, 2, 4, 7, 11}},
            new int[][] {new int[]{9, 0, 1 }, new int[] { 8, 4, 7 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{0, 1, 4, 7, 10, 11}},
            new int[][] {new int[]{4, 7, 8 }, new int[] { 0, 3, 11, 10, 9}},
            new int[][] {new int[]{4, 7, 9, 10, 11}},
            new int[][] {new int[]{9, 5, 4}},
            new int[][] {new int[]{9, 5, 4 }, new int[] { 0, 8, 3}},
            new int[][] {new int[]{0, 5, 4, 1}},
            new int[][] {new int[]{1, 3, 4, 5, 8}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 5, 4}},
            new int[][] {new int[]{3, 0, 8 }, new int[] { 1, 2, 10 }, new int[] { 4, 9, 5}},
            new int[][] {new int[]{0, 2, 4, 5, 10}},
            new int[][] {new int[]{2, 3, 4, 5, 8, 10}},
            new int[][] {new int[]{9, 5, 4 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 0, 2, 11, 8}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 4, 5, 1}},
            new int[][] {new int[]{1, 2, 4, 5, 8, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 9, 4, 5}},
            new int[][] {new int[]{4, 9, 5 }, new int[] { 0, 1, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 4, 5, 10, 11}},
            new int[][] {new int[]{5, 4, 8, 10, 11}},
            new int[][] {new int[]{5, 7, 8, 9}},
            new int[][] {new int[]{0, 3, 5, 7, 9}},
            new int[][] {new int[]{0, 1, 5, 7, 8}},
            new int[][] {new int[]{1, 5, 3, 7}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 7, 5, 9}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 7, 5, 9}},
            new int[][] {new int[]{0, 2, 5, 7, 8, 10}},
            new int[][] {new int[]{2, 3, 5, 7, 10}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 5, 7, 8, 9}},
            new int[][] {new int[]{0, 2, 5, 7, 9, 11}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 0, 1, 5, 7, 8}},
            new int[][] {new int[]{1, 2, 5, 7, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 8, 7, 5, 9}},
            new int[][] {new int[]{0, 1, 5, 7, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 5, 7, 8, 10, 11}},
            new int[][] {new int[]{5, 7, 10, 11}},
            new int[][] {new int[]{5, 6, 10}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 3, 8, 9, 1}},
            new int[][] {new int[]{1, 2, 5, 6}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{0, 2, 5, 6, 9}},
            new int[][] {new int[]{2, 3, 5, 6, 8, 9}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 10, 6, 5}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 0, 8, 2, 11}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 1, 9 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 11, 2, 8, 9, 1}},
            new int[][] {new int[]{1, 3, 5, 6, 11}},
            new int[][] {new int[]{0, 1, 5, 6, 8, 11}},
            new int[][] {new int[]{0, 3, 5, 6, 9, 11}},
            new int[][] {new int[]{5, 6, 8, 9, 11}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 5, 6, 10}},
            new int[][] {new int[]{5, 6, 10 }, new int[] { 0, 3, 4, 7}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 1, 9, 0 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 7, 4, 9, 1, 3}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{0, 3, 7, 4 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 0, 9, 2, 5, 6}},
            new int[][] {new int[]{2, 3, 4, 5, 6, 7, 9}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 5, 6, 10 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 0, 2, 11, 7, 4}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 1, 9 }, new int[] { 10, 5, 6 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 7, 4, 11, 2, 1, 9}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 3, 11, 6, 5, 1}},
            new int[][] {new int[]{0, 1, 4, 5, 6, 7, 11}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 6, 5, 9, 0, 11, 3}},
            new int[][] {new int[]{4, 5, 6, 7, 9, 11}},
            new int[][] {new int[]{4, 6, 9, 10}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 9, 10, 6, 4}},
            new int[][] {new int[]{0, 1, 4, 6, 10}},
            new int[][] {new int[]{1, 3, 4, 6, 8, 10}},
            new int[][] {new int[]{1, 2, 4, 6, 9}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 1, 2, 4, 6, 9}},
            new int[][] {new int[]{0, 2, 4, 6}},
            new int[][] {new int[]{2, 3, 4, 6, 8}},
            new int[][] {new int[]{11, 2, 3 }, new int[] { 9, 4, 10, 6}},
            new int[][] {new int[]{0, 2, 11, 8 }, new int[] { 9, 4, 6, 10}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 0, 1, 4, 6, 10}},
            new int[][] {new int[]{1, 2, 4, 6, 8, 10, 11}},
            new int[][] {new int[]{1, 3, 4, 6, 9, 11}},
            new int[][] {new int[]{0, 1, 4, 6, 8, 9, 11}},
            new int[][] {new int[]{0, 3, 4, 6, 11}},
            new int[][] {new int[]{4, 6, 8, 11}},
            new int[][] {new int[]{6, 7, 8, 9, 10}},
            new int[][] {new int[]{0, 3, 6, 7, 9, 10}},
            new int[][] {new int[]{0, 1, 6, 7, 8, 10}},
            new int[][] {new int[]{1, 3, 6, 7, 10}},
            new int[][] {new int[]{1, 2, 6, 7, 8, 9}},
            new int[][] {new int[]{0, 1, 2, 3, 6, 7, 9}},
            new int[][] {new int[]{0, 2, 6, 7, 8}},
            new int[][] {new int[]{2, 3, 6, 7}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 10, 6, 9, 7, 8}},
            new int[][] {new int[]{0, 2, 6, 7, 9, 10, 11}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 8, 7, 0, 1, 10, 6}},
            new int[][] {new int[]{1, 2, 6, 7, 10, 11}},
            new int[][] {new int[]{1, 3, 6, 7, 8, 9, 11}},
            new int[][] {new int[]{0, 1, 6, 7, 9, 11}},
            new int[][] {new int[]{0, 3, 6, 7, 8, 11}},
            new int[][] {new int[]{6, 7, 11}},
            new int[][] {new int[]{11, 7, 6}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 9, 1}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 3, 8, 9}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 2, 10 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 9, 10, 2}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 2, 3, 8, 9, 10}},
            new int[][] {new int[]{2, 3, 6, 7}},
            new int[][] {new int[]{0, 2, 6, 7, 8}},
            new int[][] {new int[]{0, 1, 9, }, new int[] { 3, 2, 6, 7}},
            new int[][] {new int[]{1, 2, 6, 7, 8, 9}},
            new int[][] {new int[]{1, 3, 6, 7, 10}},
            new int[][] {new int[]{0, 1, 6, 7, 8, 10}},
            new int[][] {new int[]{0, 3, 6, 7, 9, 10}},
            new int[][] {new int[]{6, 7, 8, 9, 10}},
            new int[][] {new int[]{4, 6, 8, 11}},
            new int[][] {new int[]{0, 3, 4, 6, 11}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 4, 11, 6}},
            new int[][] {new int[]{1, 3, 4, 6, 9, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 4, 6, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 11, 6, 4}},
            new int[][] {new int[]{0, 9, 10, 2 }, new int[] { 8, 4, 11, 6}},
            new int[][] {new int[]{2, 3, 4, 6, 9, 10, 11}},
            new int[][] {new int[]{2, 3, 4, 6, 8}},
            new int[][] {new int[]{0, 2, 4, 6}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 2, 3, 8, 4, 6}},
            new int[][] {new int[]{1, 2, 4, 6, 9}},
            new int[][] {new int[]{1, 3, 4, 6, 8, 10}},
            new int[][] {new int[]{0, 1, 4, 6, 10}},
            new int[][] {new int[]{0, 3, 4, 6, 8, 9, 10}},
            new int[][] {new int[]{4, 6, 9, 10}},
            new int[][] {new int[]{6, 7, 11 }, new int[] { 4, 5, 9}},
            new int[][] {new int[]{6, 7, 11 }, new int[] { 4, 5, 9 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 0, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 8, 3, 1, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 4, 5, 9 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 4, 5, 9 }, new int[] { 1, 2, 10 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 2, 10, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 8, 3, 2, 10, 5, 4}},
            new int[][] {new int[]{4, 5, 9 }, new int[] { 3, 2, 6, 7}},
            new int[][] {new int[]{4, 5, 9 }, new int[] { 2, 0, 8, 7, 6}},
            new int[][] {new int[]{3, 2, 6, 7 }, new int[] { 0, 1, 5, 4}},
            new int[][] {new int[]{1, 2, 4, 5, 6, 7, 8}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 1, 10, 6, 7, 3}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 6, 10, 1, 0, 8, 7}},
            new int[][] {new int[]{0, 3, 4, 5, 6, 7, 10}},
            new int[][] {new int[]{4, 5, 6, 7, 8, 10}},
            new int[][] {new int[]{5, 6, 8, 9, 11}},
            new int[][] {new int[]{0, 3, 5, 6, 9, 11}},
            new int[][] {new int[]{0, 1, 5, 6, 8, 11}},
            new int[][] {new int[]{1, 3, 5, 6, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 5, 6, 11, 8}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 0, 3, 11, 6, 5}},
            new int[][] {new int[]{0, 2, 5, 6, 8, 10, 11}},
            new int[][] {new int[]{2, 3, 5, 6, 10, 11}},
            new int[][] {new int[]{2, 3, 5, 6, 8, 9}},
            new int[][] {new int[]{0, 2, 5, 6, 9}},
            new int[][] {new int[]{0, 1, 2, 3, 5, 6, 8}},
            new int[][] {new int[]{1, 2, 5, 6}},
            new int[][] {new int[]{1, 3, 5, 6, 8, 9, 10}},
            new int[][] {new int[]{0, 1, 5, 6, 9, 10}},
            new int[][] {new int[]{0, 3, 5, 6, 8, 10}},
            new int[][] {new int[]{5, 6, 10}},
            new int[][] {new int[]{5, 7, 10, 11}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 0, 1, 9}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 3, 8, 9, 1}},
            new int[][] {new int[]{1, 2, 5, 7, 11}},
            new int[][] {new int[]{1, 2, 5, 7, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{0, 2, 5, 7, 9, 11}},
            new int[][] {new int[]{2, 3, 5, 7, 8, 9, 11}},
            new int[][] {new int[]{2, 3, 5, 7, 10}},
            new int[][] {new int[]{0, 2, 5, 7, 8, 10}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 7, 3, 2, 10, 5}},
            new int[][] {new int[]{1, 2, 5, 7, 8, 9, 10}},
            new int[][] {new int[]{1, 3, 5, 7}},
            new int[][] {new int[]{0, 1, 5, 7, 8}},
            new int[][] {new int[]{0, 3, 5, 7, 9}},
            new int[][] {new int[]{5, 7, 8, 9}},
            new int[][] {new int[]{4, 5, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 4, 5, 10, 11}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 11, 10, 4, 5}},
            new int[][] {new int[]{1, 3, 4, 5, 9, 10, 11}},
            new int[][] {new int[]{1, 2, 4, 5, 8, 11}},
            new int[][] {new int[]{0, 1, 2, 3, 4, 5, 11}},
            new int[][] {new int[]{0, 2, 4, 5, 8, 9, 11}},
            new int[][] {new int[]{2, 3, 4, 5, 9, 11}},
            new int[][] {new int[]{2, 3, 4, 5, 8, 10}},
            new int[][] {new int[]{0, 2, 4, 5, 10}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 3, 2, 10, 5, 4}},
            new int[][] {new int[]{1, 2, 4, 5, 9, 10}},
            new int[][] {new int[]{1, 3, 4, 5, 8}},
            new int[][] {new int[]{0, 1, 4, 5}},
            new int[][] {new int[]{0, 3, 4, 5, 8, 9}},
            new int[][] {new int[]{4, 5, 9}},
            new int[][] {new int[]{4, 7, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 10, 9, 4, 7, 11}},
            new int[][] {new int[]{0, 1, 4, 7, 10, 11}},
            new int[][] {new int[]{1, 3, 4, 7, 8, 10, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{0, 2, 4, 7, 11}},
            new int[][] {new int[]{2, 3, 4, 7, 8, 11}},
            new int[][] {new int[]{2, 3, 4, 7, 9, 10}},
            new int[][] {new int[]{0, 2, 4, 7, 8, 9, 10}},
            new int[][] {new int[]{0, 1, 2, 3, 4, 7, 10}},
            new int[][] {new int[]{1, 2, 4, 7, 8, 10}},
            new int[][] {new int[]{1, 3, 4, 7, 9}},
            new int[][] {new int[]{0, 1, 4, 7, 8, 9}},
            new int[][] {new int[]{0, 3, 4, 7}},
            new int[][] {new int[]{4, 7, 8}},
            new int[][] {new int[]{8, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 9, 10, 11}},
            new int[][] {new int[]{0, 1, 8, 10, 11}},
            new int[][] {new int[]{1, 3, 10, 11}},
            new int[][] {new int[]{1, 2, 8, 9, 11}},
            new int[][] {new int[]{0, 1, 2, 3, 9, 11}},
            new int[][] {new int[]{0, 2, 8, 11}},
            new int[][] {new int[]{2, 3, 11}},
            new int[][] {new int[]{2, 3, 8, 9, 10}},
            new int[][] {new int[]{0, 2, 9, 10}},
            new int[][] {new int[]{0, 1, 2, 3, 8, 10}},
            new int[][] {new int[]{1, 2, 10}},
            new int[][] {new int[]{1, 3, 8, 9}},
            new int[][] {new int[]{0, 1, 9}},
            new int[][] {new int[]{0, 3, 8}},
            new int[][] { null }
    };
    //Number of vertices per case
    public static int[] vertexCountTable = new int[]
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
        1, 1, 2, 1, 2, 2, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1,
        2, 3, 2, 2, 3, 4, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1,
        1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 2, 2, 2, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1,
        1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1,
        1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
    };
    static int[,] edgeCorners =
    {
        { 0, 1 },
        { 1, 2 },
        { 3, 2 },
        { 0, 3 },
        { 4, 5 },
        { 5, 6 },
        { 7, 6 },
        { 4, 7 },
        { 0, 4 },
        { 1, 5 },
        { 2, 6 },
        { 3, 7 }
    };
    static Vector3Int[] cornerOffsets =
    {
         new Vector3Int(0,0,0),
         new Vector3Int(0,0,1),
         new Vector3Int(1,0,1),
         new Vector3Int(1,0,0),
         new Vector3Int(0,1,0),
         new Vector3Int(0,1,1),
         new Vector3Int(1,1,1),
         new Vector3Int(1,1,0)
    };
    #endregion Tables

    public static Vector3Int[] edgeOffsets = new Vector3Int[12]
    {
        new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1),  new Vector3Int(1, 0, 0)
    };

    public static int[] edgeIndexMapping = new int[12] { 0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2 };

    public static int[,][] vertexOrderTable = new int[3, 2][]
    {
        {new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}},
        {new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}},
        {new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}}

        //{new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}},
        //{new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}},
        //{new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}}


    };

    public struct SamplePoint
    {
        public Vector3 position;
        public float value;

        public SamplePoint(Vector3 pos, float val)
        {
            position = pos;
            value = val;
        }
    }

    public struct Edge
    {
        public int[] vertices;
        public int vertexCount;
        public Edge(int vertCount)
        {
            vertices = new int[vertCount];
            vertexCount = 0;
        }

        public int AddVertexIndex(int index)
        {
            if(vertexCount < 4)
            {
                vertices[vertexCount++] = index;
                return vertexCount;
            }
            return -1;
        }
    }
    

    public static void March(Vector3 position, int size, float stepSize, List<Vector3> meshVertices, List<int> meshTriangles, List<Vector3> meshNormals)
    {
        Stopwatch sw0 = new Stopwatch();
        sw0.Start();
        int sizePlus1 = size + 1;
        SamplePoint[,,] samples = new SamplePoint[sizePlus1, sizePlus1, sizePlus1];
        Edge[,,,] edges = new Edge[sizePlus1, sizePlus1, sizePlus1, 3];
        for (int z = 0; z < sizePlus1; z++)
        {
            for (int y = 0; y < sizePlus1; y++)
            {
                for (int x = 0; x < sizePlus1; x++)
                {
                    Vector3 localPosition = new Vector3(x * stepSize, y * stepSize, z * stepSize);
                    samples[x, y, z] = new SamplePoint(localPosition, TerrainDensityField.instance.GetValue(position + localPosition));

                   edges[x, y, z, 0] = new Edge(4);
                   edges[x, y, z, 1] = new Edge(4);
                   edges[x, y, z, 2] = new Edge(4);
                }
            }
        }


        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Polygonise(size, new Vector3Int(x, y, z), samples, edges, 0.0f, meshVertices, meshTriangles, meshNormals);
                }
            }
        }

        sw0.Stop();
        UnityEngine.Debug.Log("Generation took " + sw0.ElapsedMilliseconds);
    }

    protected static void Polygonise(int sampleSize, Vector3Int pos, SamplePoint[,,] samples, Edge[,,,] edges, float isoLevel, List<Vector3> meshVertices, List<int> meshTriangles, List<Vector3> meshNormals)
    {
        int cubeCase = 0;
        if (samples[pos.x, pos.y, pos.z].value < isoLevel) { cubeCase |= 1; }
        if (samples[pos.x, pos.y, pos.z + 1].value < isoLevel) { cubeCase |= 2; }
        if (samples[pos.x + 1, pos.y, pos.z + 1].value < isoLevel) { cubeCase |= 4; }
        if (samples[pos.x + 1, pos.y, pos.z].value < isoLevel) { cubeCase |= 8; }
        if (samples[pos.x, pos.y + 1, pos.z].value < isoLevel) { cubeCase |= 16; }
        if (samples[pos.x, pos.y + 1, pos.z + 1].value < isoLevel) { cubeCase |= 32; }
        if (samples[pos.x + 1, pos.y + 1, pos.z + 1].value < isoLevel) { cubeCase |= 64; }
        if (samples[pos.x + 1, pos.y + 1, pos.z].value < isoLevel) { cubeCase |= 128; }
        if (cubeCase == 0 || cubeCase == 255)
        {
            return;
        }


        int[][] verticesData = edgeTable[cubeCase];
        for (int v = 0; v < verticesData.Length; v++)
        {
            int[] connectedEdges = verticesData[v];
            int vertexIndex = meshVertices.Count;
            Vector3 vertexPosition = new Vector3(0.0f, 0.0f, 0.0f);
            for (int e = 0; e < connectedEdges.Length; e++)
            {
                int currentEdge = connectedEdges[e];


                Vector3Int cornerPos0 = pos + cornerOffsets[edgeCorners[currentEdge, 0]];
                Vector3Int cornerPos1 = pos + cornerOffsets[edgeCorners[currentEdge, 1]];
                SamplePoint sample0 = samples[cornerPos0.x, cornerPos0.y, cornerPos0.z];
                SamplePoint sample1 = samples[cornerPos1.x, cornerPos1.y, cornerPos1.z];
                vertexPosition += sample0.position + (sample1.position - sample0.position) * ((isoLevel - sample0.value) / (sample1.value - sample0.value));

                int edgeIndex = edgeIndexMapping[currentEdge];
                //if ((pos.x == sampleSize && edgeIndex == 1) || (pos.y == sampleSize && edgeIndex == 2) || (pos.z == sampleSize && edgeIndex == 0))
                //{
                //    continue;
                //}

                Vector3Int edgePos = pos + edgeOffsets[currentEdge];
                int edgeVertexCount = edges[edgePos.x, edgePos.y, edgePos.z, edgeIndex].AddVertexIndex(vertexIndex); 
                Edge edge = edges[edgePos.x, edgePos.y, edgePos.z, edgeIndex];

                if (edgeVertexCount == 4)
                {
                    int[] vertexIndices = vertexOrderTable[edgeIndex, sample0.value < isoLevel ? 1 : 0];
                    meshTriangles.Add(edge.vertices[vertexIndices[0]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[1]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[2]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[3]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[2]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[1]]);
                }
            }

            meshVertices.Add(vertexPosition / connectedEdges.Length);
        }

    }
}
 */

public class DualMarchingCubes
{
    #region Enums
    //     5----------6
    //    /|         /|
    //   / |        / |
    //  4----------7  |
    //  |  |       |  |
    //  |  1-------|--2     
    //  | /        | /      y z
    //  |/         |/       |/
    //  0----------3        +--X
    public enum CubeCorner
    {
        LEFT_BOTTOM_BACK,
        LEFT_BOTTOM_FRONT,
        RIGHT_BOTTOM_FRONT,
        RIGHT_BOTTOM_BACK,
        LEFT_TOP_BACK,
        LEFT_TOP_FRONT,
        RIGHT_TOP_FRONT,
        RIGHT_TOP_BACK,
    }



    //          
    //      #----5-----#
    //     /|         /|
    //    4 9        6 |
    //   /  |       /  10
    //  #----7-----#   |
    //  |   |      |   |
    //  8   #----1-|---#     
    //  |  /       11 /      
    //  | 0        | 2      y z 
    //  |/         |/       |/
    //  #----3-----#        +--X
    //       
    public enum CubeEdge
    {
        LEFT_BOTTOM,
        FRONT_BOTTOM,
        RIGHT_BOTTOM,
        BACK_BOTTOM,
        LEFT_TOP,
        FRONT_TOP,
        RIGHT_TOP,
        BACK_TOP,
        LEFT_BACK,
        LEFT_FRONT,
        RIGHT_FRONT,
        RIGHT_BACK
    }
    #endregion Enums

    #region Tables
    //Table taken from https://stackoverflow.com/questions/16638711/dual-marching-cubes-table
    static int[][][] edgeTable = new int[][][]
    {
            new int[][] { null },
            new int[][] {new int[]{0, 8, 3}},
            new int[][] {new int[]{0, 1, 9}},
            new int[][] {new int[]{1, 3, 8, 9}},
            new int[][] {new int[]{1, 2, 10}},
            new int[][] { new int[] { 0, 8, 3 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{9, 0, 2, 10}},
            new int[][] {new int[]{10, 2, 3, 8, 9}},
            new int[][] {new int[]{3, 11, 2}},
            new int[][] {new int[]{0, 8, 11, 2}},
            new int[][] {new int[]{1, 9, 0 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{1, 2, 8, 9, 11}},
            new int[][] {new int[]{1, 3, 10, 11}},
            new int[][] {new int[]{0, 1, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 9, 10, 11}},
            new int[][] {new int[]{8, 9, 10, 11}},
            new int[][] {new int[]{4, 7, 8}},
            new int[][] {new int[]{0, 3, 4, 7}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 4, 7}},
            new int[][] {new int[]{1, 3, 4, 7, 9}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 4, 7}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 7, 4}},
            new int[][] {new int[]{0, 2, 10, 9 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{2, 3, 4, 7, 9, 10}},
            new int[][] {new int[]{8, 4, 7 }, new int[] { 3, 11, 2}},
            new int[][] {new int[]{0, 2, 4, 7, 11}},
            new int[][] {new int[]{9, 0, 1 }, new int[] { 8, 4, 7 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{0, 1, 4, 7, 10, 11}},
            new int[][] {new int[]{4, 7, 8 }, new int[] { 0, 3, 11, 10, 9}},
            new int[][] {new int[]{4, 7, 9, 10, 11}},
            new int[][] {new int[]{9, 5, 4}},
            new int[][] {new int[]{9, 5, 4 }, new int[] { 0, 8, 3}},
            new int[][] {new int[]{0, 5, 4, 1}},
            new int[][] {new int[]{1, 3, 4, 5, 8}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 5, 4}},
            new int[][] {new int[]{3, 0, 8 }, new int[] { 1, 2, 10 }, new int[] { 4, 9, 5}},
            new int[][] {new int[]{0, 2, 4, 5, 10}},
            new int[][] {new int[]{2, 3, 4, 5, 8, 10}},
            new int[][] {new int[]{9, 5, 4 }, new int[] { 2, 3, 11}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 0, 2, 11, 8}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 4, 5, 1}},
            new int[][] {new int[]{1, 2, 4, 5, 8, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 9, 4, 5}},
            new int[][] {new int[]{4, 9, 5 }, new int[] { 0, 1, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 4, 5, 10, 11}},
            new int[][] {new int[]{5, 4, 8, 10, 11}},
            new int[][] {new int[]{5, 7, 8, 9}},
            new int[][] {new int[]{0, 3, 5, 7, 9}},
            new int[][] {new int[]{0, 1, 5, 7, 8}},
            new int[][] {new int[]{1, 5, 3, 7}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 7, 5, 9}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 7, 5, 9}},
            new int[][] {new int[]{0, 2, 5, 7, 8, 10}},
            new int[][] {new int[]{2, 3, 5, 7, 10}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 5, 7, 8, 9}},
            new int[][] {new int[]{0, 2, 5, 7, 9, 11}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 0, 1, 5, 7, 8}},
            new int[][] {new int[]{1, 2, 5, 7, 11}},
            new int[][] {new int[]{3, 11, 10, 1 }, new int[] { 8, 7, 5, 9}},
            new int[][] {new int[]{0, 1, 5, 7, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 5, 7, 8, 10, 11}},
            new int[][] {new int[]{5, 7, 10, 11}},
            new int[][] {new int[]{5, 6, 10}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 3, 8, 9, 1}},
            new int[][] {new int[]{1, 2, 5, 6}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{0, 2, 5, 6, 9}},
            new int[][] {new int[]{2, 3, 5, 6, 8, 9}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 10, 6, 5}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 0, 8, 2, 11}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 1, 9 }, new int[] { 10, 5, 6}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 11, 2, 8, 9, 1}},
            new int[][] {new int[]{1, 3, 5, 6, 11}},
            new int[][] {new int[]{0, 1, 5, 6, 8, 11}},
            new int[][] {new int[]{0, 3, 5, 6, 9, 11}},
            new int[][] {new int[]{5, 6, 8, 9, 11}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 5, 6, 10}},
            new int[][] {new int[]{5, 6, 10 }, new int[] { 0, 3, 4, 7}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 1, 9, 0 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 7, 4, 9, 1, 3}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{0, 3, 7, 4 }, new int[] { 1, 2, 6, 5}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 0, 9, 2, 5, 6}},
            new int[][] {new int[]{2, 3, 4, 5, 6, 7, 9}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 5, 6, 10 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 0, 2, 11, 7, 4}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 0, 1, 9 }, new int[] { 10, 5, 6 }, new int[] { 8, 7, 4}},
            new int[][] {new int[]{10, 5, 6 }, new int[] { 7, 4, 11, 2, 1, 9}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 3, 11, 6, 5, 1}},
            new int[][] {new int[]{0, 1, 4, 5, 6, 7, 11}},
            new int[][] {new int[]{8, 7, 4 }, new int[] { 6, 5, 9, 0, 11, 3}},
            new int[][] {new int[]{4, 5, 6, 7, 9, 11}},
            new int[][] {new int[]{4, 6, 9, 10}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 9, 10, 6, 4}},
            new int[][] {new int[]{0, 1, 4, 6, 10}},
            new int[][] {new int[]{1, 3, 4, 6, 8, 10}},
            new int[][] {new int[]{1, 2, 4, 6, 9}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 1, 2, 4, 6, 9}},
            new int[][] {new int[]{0, 2, 4, 6}},
            new int[][] {new int[]{2, 3, 4, 6, 8}},
            new int[][] {new int[]{11, 2, 3 }, new int[] { 9, 4, 10, 6}},
            new int[][] {new int[]{0, 2, 11, 8 }, new int[] { 9, 4, 6, 10}},
            new int[][] {new int[]{2, 3, 11 }, new int[] { 0, 1, 4, 6, 10}},
            new int[][] {new int[]{1, 2, 4, 6, 8, 10, 11}},
            new int[][] {new int[]{1, 3, 4, 6, 9, 11}},
            new int[][] {new int[]{0, 1, 4, 6, 8, 9, 11}},
            new int[][] {new int[]{0, 3, 4, 6, 11}},
            new int[][] {new int[]{4, 6, 8, 11}},
            new int[][] {new int[]{6, 7, 8, 9, 10}},
            new int[][] {new int[]{0, 3, 6, 7, 9, 10}},
            new int[][] {new int[]{0, 1, 6, 7, 8, 10}},
            new int[][] {new int[]{1, 3, 6, 7, 10}},
            new int[][] {new int[]{1, 2, 6, 7, 8, 9}},
            new int[][] {new int[]{0, 1, 2, 3, 6, 7, 9}},
            new int[][] {new int[]{0, 2, 6, 7, 8}},
            new int[][] {new int[]{2, 3, 6, 7}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 10, 6, 9, 7, 8}},
            new int[][] {new int[]{0, 2, 6, 7, 9, 10, 11}},
            new int[][] {new int[]{3, 11, 2 }, new int[] { 8, 7, 0, 1, 10, 6}},
            new int[][] {new int[]{1, 2, 6, 7, 10, 11}},
            new int[][] {new int[]{1, 3, 6, 7, 8, 9, 11}},
            new int[][] {new int[]{0, 1, 6, 7, 9, 11}},
            new int[][] {new int[]{0, 3, 6, 7, 8, 11}},
            new int[][] {new int[]{6, 7, 11}},
            new int[][] {new int[]{11, 7, 6}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 9, 1}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 3, 8, 9}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 2, 10 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 9, 10, 2}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 2, 3, 8, 9, 10}},
            new int[][] {new int[]{2, 3, 6, 7}},
            new int[][] {new int[]{0, 2, 6, 7, 8}},
            new int[][] {new int[]{0, 1, 9, }, new int[] { 3, 2, 6, 7}},
            new int[][] {new int[]{1, 2, 6, 7, 8, 9}},
            new int[][] {new int[]{1, 3, 6, 7, 10}},
            new int[][] {new int[]{0, 1, 6, 7, 8, 10}},
            new int[][] {new int[]{0, 3, 6, 7, 9, 10}},
            new int[][] {new int[]{6, 7, 8, 9, 10}},
            new int[][] {new int[]{4, 6, 8, 11}},
            new int[][] {new int[]{0, 3, 4, 6, 11}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 4, 11, 6}},
            new int[][] {new int[]{1, 3, 4, 6, 9, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 8, 4, 6, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 0, 3, 11, 6, 4}},
            new int[][] {new int[]{0, 9, 10, 2 }, new int[] { 8, 4, 11, 6}},
            new int[][] {new int[]{2, 3, 4, 6, 9, 10, 11}},
            new int[][] {new int[]{2, 3, 4, 6, 8}},
            new int[][] {new int[]{0, 2, 4, 6}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 2, 3, 8, 4, 6}},
            new int[][] {new int[]{1, 2, 4, 6, 9}},
            new int[][] {new int[]{1, 3, 4, 6, 8, 10}},
            new int[][] {new int[]{0, 1, 4, 6, 10}},
            new int[][] {new int[]{0, 3, 4, 6, 8, 9, 10}},
            new int[][] {new int[]{4, 6, 9, 10}},
            new int[][] {new int[]{6, 7, 11 }, new int[] { 4, 5, 9}},
            new int[][] {new int[]{6, 7, 11 }, new int[] { 4, 5, 9 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 1, 0, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 8, 3, 1, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 4, 5, 9 }, new int[] { 1, 2, 10}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 4, 5, 9 }, new int[] { 1, 2, 10 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 0, 2, 10, 5, 4}},
            new int[][] {new int[]{11, 7, 6 }, new int[] { 8, 3, 2, 10, 5, 4}},
            new int[][] {new int[]{4, 5, 9 }, new int[] { 3, 2, 6, 7}},
            new int[][] {new int[]{4, 5, 9 }, new int[] { 2, 0, 8, 7, 6}},
            new int[][] {new int[]{3, 2, 6, 7 }, new int[] { 0, 1, 5, 4}},
            new int[][] {new int[]{1, 2, 4, 5, 6, 7, 8}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 1, 10, 6, 7, 3}},
            new int[][] {new int[]{9, 4, 5 }, new int[] { 6, 10, 1, 0, 8, 7}},
            new int[][] {new int[]{0, 3, 4, 5, 6, 7, 10}},
            new int[][] {new int[]{4, 5, 6, 7, 8, 10}},
            new int[][] {new int[]{5, 6, 8, 9, 11}},
            new int[][] {new int[]{0, 3, 5, 6, 9, 11}},
            new int[][] {new int[]{0, 1, 5, 6, 8, 11}},
            new int[][] {new int[]{1, 3, 5, 6, 11}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 5, 6, 11, 8}},
            new int[][] {new int[]{1, 2, 10 }, new int[] { 9, 0, 3, 11, 6, 5}},
            new int[][] {new int[]{0, 2, 5, 6, 8, 10, 11}},
            new int[][] {new int[]{2, 3, 5, 6, 10, 11}},
            new int[][] {new int[]{2, 3, 5, 6, 8, 9}},
            new int[][] {new int[]{0, 2, 5, 6, 9}},
            new int[][] {new int[]{0, 1, 2, 3, 5, 6, 8}},
            new int[][] {new int[]{1, 2, 5, 6}},
            new int[][] {new int[]{1, 3, 5, 6, 8, 9, 10}},
            new int[][] {new int[]{0, 1, 5, 6, 9, 10}},
            new int[][] {new int[]{0, 3, 5, 6, 8, 10}},
            new int[][] {new int[]{5, 6, 10}},
            new int[][] {new int[]{5, 7, 10, 11}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 0, 1, 9}},
            new int[][] {new int[]{5, 7, 10, 11 }, new int[] { 3, 8, 9, 1}},
            new int[][] {new int[]{1, 2, 5, 7, 11}},
            new int[][] {new int[]{1, 2, 5, 7, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{0, 2, 5, 7, 9, 11}},
            new int[][] {new int[]{2, 3, 5, 7, 8, 9, 11}},
            new int[][] {new int[]{2, 3, 5, 7, 10}},
            new int[][] {new int[]{0, 2, 5, 7, 8, 10}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 7, 3, 2, 10, 5}},
            new int[][] {new int[]{1, 2, 5, 7, 8, 9, 10}},
            new int[][] {new int[]{1, 3, 5, 7}},
            new int[][] {new int[]{0, 1, 5, 7, 8}},
            new int[][] {new int[]{0, 3, 5, 7, 9}},
            new int[][] {new int[]{5, 7, 8, 9}},
            new int[][] {new int[]{4, 5, 8, 10, 11}},
            new int[][] {new int[]{0, 3, 4, 5, 10, 11}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 11, 10, 4, 5}},
            new int[][] {new int[]{1, 3, 4, 5, 9, 10, 11}},
            new int[][] {new int[]{1, 2, 4, 5, 8, 11}},
            new int[][] {new int[]{0, 1, 2, 3, 4, 5, 11}},
            new int[][] {new int[]{0, 2, 4, 5, 8, 9, 11}},
            new int[][] {new int[]{2, 3, 4, 5, 9, 11}},
            new int[][] {new int[]{2, 3, 4, 5, 8, 10}},
            new int[][] {new int[]{0, 2, 4, 5, 10}},
            new int[][] {new int[]{0, 1, 9 }, new int[] { 8, 3, 2, 10, 5, 4}},
            new int[][] {new int[]{1, 2, 4, 5, 9, 10}},
            new int[][] {new int[]{1, 3, 4, 5, 8}},
            new int[][] {new int[]{0, 1, 4, 5}},
            new int[][] {new int[]{0, 3, 4, 5, 8, 9}},
            new int[][] {new int[]{4, 5, 9}},
            new int[][] {new int[]{4, 7, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 8 }, new int[] { 10, 9, 4, 7, 11}},
            new int[][] {new int[]{0, 1, 4, 7, 10, 11}},
            new int[][] {new int[]{1, 3, 4, 7, 8, 10, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11}},
            new int[][] {new int[]{1, 2, 4, 7, 9, 11 }, new int[] { 0, 3, 8}},
            new int[][] {new int[]{0, 2, 4, 7, 11}},
            new int[][] {new int[]{2, 3, 4, 7, 8, 11}},
            new int[][] {new int[]{2, 3, 4, 7, 9, 10}},
            new int[][] {new int[]{0, 2, 4, 7, 8, 9, 10}},
            new int[][] {new int[]{0, 1, 2, 3, 4, 7, 10}},
            new int[][] {new int[]{1, 2, 4, 7, 8, 10}},
            new int[][] {new int[]{1, 3, 4, 7, 9}},
            new int[][] {new int[]{0, 1, 4, 7, 8, 9}},
            new int[][] {new int[]{0, 3, 4, 7}},
            new int[][] {new int[]{4, 7, 8}},
            new int[][] {new int[]{8, 9, 10, 11}},
            new int[][] {new int[]{0, 3, 9, 10, 11}},
            new int[][] {new int[]{0, 1, 8, 10, 11}},
            new int[][] {new int[]{1, 3, 10, 11}},
            new int[][] {new int[]{1, 2, 8, 9, 11}},
            new int[][] {new int[]{0, 1, 2, 3, 9, 11}},
            new int[][] {new int[]{0, 2, 8, 11}},
            new int[][] {new int[]{2, 3, 11}},
            new int[][] {new int[]{2, 3, 8, 9, 10}},
            new int[][] {new int[]{0, 2, 9, 10}},
            new int[][] {new int[]{0, 1, 2, 3, 8, 10}},
            new int[][] {new int[]{1, 2, 10}},
            new int[][] {new int[]{1, 3, 8, 9}},
            new int[][] {new int[]{0, 1, 9}},
            new int[][] {new int[]{0, 3, 8}},
            new int[][] { null }
    };

    //Indices of the corners that connect a certain edge
    static int[,] edgeCorners =
    {
        { 0, 1 },
        { 1, 2 },
        { 3, 2 },
        { 0, 3 },
        { 4, 5 },
        { 5, 6 },
        { 7, 6 },
        { 4, 7 },
        { 0, 4 },
        { 1, 5 },
        { 2, 6 },
        { 3, 7 }
    };

    //Offsets of a certain corner from the cell origin (corner 0)
    static Vector3Int[] cornerOffsets =
    {
         new Vector3Int(0,0,0),
         new Vector3Int(0,0,1),
         new Vector3Int(1,0,1),
         new Vector3Int(1,0,0),
         new Vector3Int(0,1,0),
         new Vector3Int(0,1,1),
         new Vector3Int(1,1,1),
         new Vector3Int(1,1,0)
    };

    //Cell offsets to get a certain edge in the current cell (Edges 0, 3 and 8 are part of the current cell, all other edges are part of a neighboring cell in positive x, y or z direction)
    static Vector3Int[] edgeOffsets = new Vector3Int[12]
    {
        new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1),  new Vector3Int(1, 0, 0)
    };

    //Index of an edge mapped to the corresponding edge index in a neighboring cell (always 0 to 2)
    static int[] edgeIndexMapping = new int[12] 
    {
        0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2
    };

    //Vertex order to generate a triangle for each of the 3 edges/axis in a cell, depending on if they need to be flipped or not
    static int[,][] vertexOrderTable = new int[3, 2][]
    {
        {new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}},
        {new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}},
        {new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}}

        //{new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}},
        //{new int[4]{0, 2, 1, 3}, new int[4]{0, 1, 2, 3}},
        //{new int[4]{0, 1, 2, 3}, new int[4]{0, 2, 1, 3}}
    };
    #endregion Tables

    #region Structs
    public struct SamplePoint
    {
        public Vector3 position;
        public float value;

        public SamplePoint(Vector3 pos, float val)
        {
            position = pos;
            value = val;
        }
    }

    public struct Edge
    {
        public int[] vertices;
        public int vertexCount;
        public Edge(int vertCount)
        {
            vertices = new int[vertCount];
            vertexCount = 0;
        }

        public int AddVertexIndex(int index)
        {
            if (vertexCount < 4)
            {
                vertices[vertexCount++] = index;
                return vertexCount;
            }
            return -1;
        }
    }
    #endregion Structs
   
    public static void March(Vector3Int position, int size, int stepSize, List<Vector3> meshVertices, List<int> meshTriangles, List<Vector3> meshNormals)
    {
        Stopwatch sw0 = new Stopwatch();
        Stopwatch sw1 = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        sw0.Start();
        sw1.Start();
        int sampleSize = size + 1;
        SamplePoint[,,] samples = new SamplePoint[sampleSize, sampleSize, sampleSize];
        Edge[,,,] edges = new Edge[sampleSize, sampleSize, sampleSize, 3];
        for (int z = 0; z < sampleSize; z++)
        {
            for (int y = 0; y < sampleSize; y++)
            {
                for (int x = 0; x < sampleSize; x++)
                {
                    Vector3Int localPosition = new Vector3Int(x * stepSize, y * stepSize, z * stepSize);
                    //samples[x, y, z] = new SamplePoint(localPosition, ProceduralTerrain.instance.GetValue(position + localPosition));   
                    edges[x, y, z, 0] = new Edge(4);
                    edges[x, y, z, 1] = new Edge(4);
                    edges[x, y, z, 2] = new Edge(4);
                }
            }
        }

        sw1.Stop();
        sw2.Start();

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Polygonise(size, new Vector3Int(x, y, z), samples, edges, 0.0f, meshVertices, meshTriangles, meshNormals);
                }
            }
        }
        sw2.Stop();
        sw0.Stop();
        //UnityEngine.Debug.Log(position + ": " + sw0.ElapsedMilliseconds + "ms," + sw1.ElapsedMilliseconds + "ms," + sw2.ElapsedMilliseconds + "ms");
    }

    protected static void Polygonise(int sampleSize, Vector3Int pos, SamplePoint[,,] samples, Edge[,,,] edges, float isoLevel, List<Vector3> meshVertices, List<int> meshTriangles, List<Vector3> meshNormals)
    {
        int cubeCase = 0;
        if (samples[pos.x, pos.y, pos.z].value < isoLevel) { cubeCase |= 1; }
        if (samples[pos.x, pos.y, pos.z + 1].value < isoLevel) { cubeCase |= 2; }
        if (samples[pos.x + 1, pos.y, pos.z + 1].value < isoLevel) { cubeCase |= 4; }
        if (samples[pos.x + 1, pos.y, pos.z].value < isoLevel) { cubeCase |= 8; }
        if (samples[pos.x, pos.y + 1, pos.z].value < isoLevel) { cubeCase |= 16; }
        if (samples[pos.x, pos.y + 1, pos.z + 1].value < isoLevel) { cubeCase |= 32; }
        if (samples[pos.x + 1, pos.y + 1, pos.z + 1].value < isoLevel) { cubeCase |= 64; }
        if (samples[pos.x + 1, pos.y + 1, pos.z].value < isoLevel) { cubeCase |= 128; }
        if (cubeCase == 0 || cubeCase == 255)
        {
            return;
        }


        int[][] verticesData = edgeTable[cubeCase];
        for (int v = 0; v < verticesData.Length; v++)
        {
            int[] connectedEdges = verticesData[v];
            int vertexIndex = meshVertices.Count;
            Vector3 vertexPosition = new Vector3(0.0f, 0.0f, 0.0f);
            for (int e = 0; e < connectedEdges.Length; e++)
            {
                int currentEdge = connectedEdges[e];


                Vector3Int cornerPos0 = pos + cornerOffsets[edgeCorners[currentEdge, 0]];
                Vector3Int cornerPos1 = pos + cornerOffsets[edgeCorners[currentEdge, 1]];
                SamplePoint sample0 = samples[cornerPos0.x, cornerPos0.y, cornerPos0.z];
                SamplePoint sample1 = samples[cornerPos1.x, cornerPos1.y, cornerPos1.z];
                vertexPosition += sample0.position + (sample1.position - sample0.position) * ((isoLevel - sample0.value) / (sample1.value - sample0.value));

                int edgeIndex = edgeIndexMapping[currentEdge];
                //if ((pos.x == sampleSize && edgeIndex == 1) || (pos.y == sampleSize && edgeIndex == 2) || (pos.z == sampleSize && edgeIndex == 0))
                //{
                //    continue;
                //}

                Vector3Int edgePos = pos + edgeOffsets[currentEdge];
                int edgeVertexCount = edges[edgePos.x, edgePos.y, edgePos.z, edgeIndex].AddVertexIndex(vertexIndex); 
                Edge edge = edges[edgePos.x, edgePos.y, edgePos.z, edgeIndex];

                if (edgeVertexCount == 4)
                {
                    int[] vertexIndices = vertexOrderTable[edgeIndex, sample0.value < isoLevel ? 1 : 0];
                    meshTriangles.Add(edge.vertices[vertexIndices[0]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[1]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[2]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[3]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[2]]);
                    meshTriangles.Add(edge.vertices[vertexIndices[1]]);
                }
            }

            meshVertices.Add(vertexPosition / connectedEdges.Length);
        }

    }
}