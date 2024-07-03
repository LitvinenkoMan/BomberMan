using System;
using System.Collections.Generic;
using MonoBehaviours.GroundSectionSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GroundSectionsCreator : EditorWindow
    {
        
        // Sections
        public GameObject LevelDataHolder;
        public GroundSection CurrentSelectedSection;
        public GameObject SectionPrefab;
        private int SectionID = 0;
        public byte LevelWidth = 16;
        public byte LevelHeight = 16;


        // Obstacles
        public GameObject ObstaclePrefab;


        [MenuItem("Tools/Ground Section Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GroundSectionsCreator));
        }

        private void OnGUI()
        {
            GUILayout.Label("Section Spawning", EditorStyles.boldLabel);
            LevelDataHolder =
                EditorGUILayout.ObjectField("Map Holder", LevelDataHolder, typeof(GameObject),
                        true)
                    as GameObject;
            CurrentSelectedSection =
                EditorGUILayout.ObjectField("Current selected Section", CurrentSelectedSection, typeof(GroundSection),
                        true)
                    as GroundSection;
            SectionPrefab =
                EditorGUILayout.ObjectField("Prefab to clone", SectionPrefab, typeof(GameObject), false) as
                    GameObject;
            SectionID = (byte)EditorGUILayout.IntField("Section ID", SectionID);
            LevelWidth = (byte)EditorGUILayout.IntField("Level width", LevelWidth);
            LevelHeight = (byte)EditorGUILayout.IntField("Level height", LevelHeight);

            if (GUILayout.Button("Create Level"))
            {
                CreateMap();
            }
            
            
            GUILayout.Label("\nObstacle settings", EditorStyles.boldLabel);
            
            ObstaclePrefab =
                EditorGUILayout.ObjectField("Obstacle to add", SectionPrefab, typeof(GameObject), false) as
                    GameObject;
            
            
            if (GUILayout.Button("Add obstacles to Selected sections"))
            {
                // Add obstacles
            }
        }

        private void CreateMap()
        {
            GroundSection[][] groundSections = new GroundSection[LevelWidth][];
            for (int index = 0; index < LevelHeight; index++)
            {
                groundSections[index] = new GroundSection[LevelHeight];
            }

            for (int i = 0; i < LevelWidth; i++)
            {
                for (int j = 0; j < LevelHeight; j++)
                {
                    GameObject newSection = Instantiate(SectionPrefab, new Vector3(i, 0, j),
                        SectionPrefab.transform.rotation, LevelDataHolder.transform);

                    groundSections[i][j] = newSection.GetComponent<GroundSection>();
                }
            }
            
            for (int i = 0; i < LevelWidth; i++)
            {
                for (int j = 0; j < LevelHeight; j++)
                {
                    if (j + 1 < LevelHeight) groundSections[i][j].ConnectedSections.upperSection = groundSections[i][j + 1];
                    if (j - 1 >= 0) groundSections[i][j].ConnectedSections.lowerSection = groundSections[i][j - 1];
                    if (i + 1 < LevelWidth)groundSections[i][j].ConnectedSections.rightSection = groundSections[i + 1][j];
                    if (i - 1 >= 0)groundSections[i][j].ConnectedSections.leftSection = groundSections[i - 1][j];
                }
            }
        }
    }
}
