using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoBreak
{
    public class SpawnArea : CBEntity
    {
        private float colliderYOffset;
        private new MeshRenderer renderer;
        private new Collider collider;
        //private Dictionary<PlayerController, Vector3> playerSpawnLocations = new Dictionary<PlayerController, Vector3>();

        

        protected override void Awake()
        {
            base.Awake();

            collider = GetComponent<Collider>();
            colliderYOffset = collider.bounds.size.y / 2f;
            renderer = GetComponent<MeshRenderer>();
        }

        protected void Start()
        {
        }

        protected override void ResetObjectToDefault()
        {
            gameObject.SetActive(true);
        }

        protected override void SaveDefaultProperties()
        {

        }

        public override void StartPlayState()
        {
            base.StartPlayState();

            gameObject.SetActive(false);
        }

        public override void StartTacticalState()
        {
            base.StartTacticalState();


        }

        public void UpdateSpawnLocation(PlayerController pc, Vector3 point)
        {
            /*if (playerSpawnLocations.ContainsKey(pc))
            {
                playerSpawnLocations[pc] = point;
            }*/
        }

        public void AttachPlayerToSpawnArea(PlayerController pc)
        {
            //Vector3 pos = GetNewDefaultSpawnPosition();
            //AttachPlayerToSpawnArea(pc, pos);
        }

        public void AttachPlayerToSpawnArea(PlayerController pc, Vector3 spawnPoint)
        {
            /*if (playerSpawnLocations.ContainsKey(pc))
                return;

            playerSpawnLocations[pc] = spawnPoint;*/
        }

        private Vector3 GetNewDefaultSpawnPosition()
        {
            //todo if this is how the spawn positions are defaulted in final design then it should 
            //choose random or predictable points that are inside the bounds of the area and dont collide with other 
            //player objects existing spawn points

            /*Vector3 spawnPoint = transform.position - new Vector3(0, colliderYOffset, 0);
            foreach (Vector3 existingSpawn in playerSpawnLocations.Values)
            {
                if (Vector3.Distance(spawnPoint, existingSpawn) < 1)
                {
                    spawnPoint.x += 1;
                }
            }

            return spawnPoint;*/

            return new Vector3();
        }

        public void RespawnPlayer(PlayerController pc)
        {
            /*Vector3 spawnPosition;
            if (playerSpawnLocations.TryGetValue(pc, out spawnPosition))
            {
                pc.Respawn(spawnPosition);
            }*/
        }

        public void RemovePlayer(PlayerController pc)
        {
            /*if (playerSpawnLocations.ContainsKey(pc))
            {
                playerSpawnLocations.Remove(pc);
            } else
            {
                Debug.LogWarning("Tried to remove a player from SpawnArea that wasn't registered");
            }*/
        }
    }
}
