using UnityEngine;

namespace GridSystem.GridItems
{
    public class Wall : GridItem
    {
        #if UNITY_EDITOR
        public void InitialiseWall(GameObject newPelletPrefab, GameObject newBonusItemPrefab)
        {
            Initialise(Indices);
            Display.color = Color.black;
            pelletPrefab = newPelletPrefab;
            bonusItemPrefab = newBonusItemPrefab;
        }
    
        [ContextMenu(nameof(ChangeToBlank))]
        private void ChangeToBlank()
        {
            var blank = gameObject.AddComponent<Blank>();
            blank.InitialiseBlank(pelletPrefab, bonusItemPrefab);
            RemoveSelf();
        }
        #endif
    }
}
