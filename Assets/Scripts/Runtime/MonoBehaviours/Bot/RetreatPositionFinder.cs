using Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RetreatPositionFinder 
{
    
    private bool PointInBlackList(List<Vector3> blackList, Vector3 point)
    {
        Vector3 checkingPoint = new Vector3(point.x, 0, point.z);

        foreach (Vector3 blackListPoint in blackList)
        {
            Vector3 blackPoint = new Vector3(blackListPoint.x, 0, blackListPoint.z);

            if (checkingPoint == blackPoint) return true;
        }
        return false;
    }

    public List<Vector3> GenerateBlacklistPositions(byte explosionRange, Vector3 bombPos)
    {
        var blacklist = new List<Vector3> { bombPos };
        for (int i = 1; i <= explosionRange; i++)
        {
            blacklist.Add(bombPos + new Vector3(i, 0, 0));
            blacklist.Add(bombPos + new Vector3(-i, 0, 0));
            blacklist.Add(bombPos + new Vector3(0, 0, i));
            blacklist.Add(bombPos + new Vector3(0, 0, -i));
        }
        return blacklist;
    }

    public List<Vector3> GeneratePossiblePositions(byte explosionRange, List<Vector3> blacklist, Vector3 spawnedBombPos)
    {
        var possiblePositions = new List<Vector3>();
        float centerX = spawnedBombPos.x;
        float centerZ = spawnedBombPos.z;

        for (int x = -explosionRange - 1; x <= explosionRange + 1; x++)
        {
            for (int z = -explosionRange - 1; z <= explosionRange + 1; z++)
            {
                Vector3 point = new Vector3(centerX + x, 0, centerZ + z);

                if (!PointInBlackList(blacklist, point))
                {
                    possiblePositions.Add(point);
                }
            }
        }
        if (possiblePositions.Count > 0)
        {
            return possiblePositions;
        }
        else
        {
            Debug.LogError("GeneratePossiblePositions: list of possible positions is null");
            return null;
        }
    }
    public List<Vector3> FindAvailablePosForRetreat(List<Vector3> possiblePos, List<Vector3> blacklist, NavMeshAgent agent)
    {
        if (possiblePos.Count == 0)
        {
            Debug.LogError(agent.gameObject.name + " FindAvailablePosForRetreat: parametr 'possiblePos' is null");
            return null;
        }
        Vector3[] sideOffsets = { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
        var availablePositions = new List<Vector3>();

        foreach (Vector3 point in possiblePos)
        {
            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(hit.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    availablePositions.Add(hit.position);

                    // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                    foreach (var offset in sideOffsets)
                    {
                        var sidePos = hit.position + offset;
                        agent.CalculatePath(sidePos, path);
                        if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, sidePos))
                        {
                            availablePositions.Add(sidePos);
                        }
                    }
                }
                else continue;
            }
        }
        if (availablePositions.Count > 0)
        {
            return availablePositions;
        }
        else
        {
            Debug.LogError(agent.gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
            return null;
        }
    }
}
