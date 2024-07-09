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
                EditorGUILayout.ObjectField("Obstacle to add", ObstaclePrefab, typeof(GameObject), false) as
                    GameObject;
            
            
            if (GUILayout.Button("Add obstacles to Selected sections"))
            {
                AddObstacle();
            }
            if (GUILayout.Button("Fix Obstacle"))
            {
                SetLostObstacleToNearestSection();
            }
        }

        private void SetLostObstacleToNearestSection()
        {
            var mapData = LevelDataHolder.GetComponent<LevelSectionsDataHolder>();
            foreach (var gameObject in Selection.gameObjects)
            {
                Obstacle obstacle;
                gameObject.TryGetComponent(out obstacle);
                if (obstacle)
                {
                    var obstaclePos = obstacle.transform.position;
                    GroundSection nearestSection = null;
                    float distance = 99999999;
                    foreach (var section in mapData.sections)
                    {
                        if (Vector3.Distance(obstaclePos, section.transform.position) < distance)
                        {
                            distance = Vector3.Distance(obstaclePos, section.transform.position);
                            nearestSection = section;
                        }
                    }
                    nearestSection.AddObstacle(obstacle);
                }
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
                    GameObject newSection = Instantiate(SectionPrefab, Vector3.zero, 
                        SectionPrefab.transform.rotation, LevelDataHolder.transform);
                    newSection.name += "_" + SectionID; 
                    GroundSection instancedSection = newSection.GetComponent<GroundSection>();
                    instancedSection.SetNewSectionPosition(new Vector3(i, 0, j));

                    groundSections[i][j] = instancedSection;
                    SectionID++;
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

        private void AddObstacle()
        {
            if (!ObstaclePrefab)
            {
                Debug.LogError("Could not add obstacle to Section, because Obstacle prefab is not set");
                return;
            }
            
            foreach (var gameObject in Selection.gameObjects)
            {
                GroundSection section;
                gameObject.TryGetComponent(out section);
                if (section && !section.PlacedObstacle)
                {
                    Obstacle obstacle = Instantiate(ObstaclePrefab).GetComponent<Obstacle>();
                    obstacle.transform.SetParent(section.transform);
                    section.SetNewSectionPosition(section.transform.position);
                    section.AddObstacle(obstacle);
                }
            }
        }
    }
}
