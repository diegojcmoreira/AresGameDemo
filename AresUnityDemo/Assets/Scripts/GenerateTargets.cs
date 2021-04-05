// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// // Generate Game Scene
// public class GenerateScene : MonoBehaviour{

//     public GameObject enemy;
//     public GameObject beginText;
//     public int yPos;
//     private int enemyNumber;

//     // Start is called before the first frame update
//     void Start(){
//         enemyNumber = 5;
//         Destroy(beginText);
//         StartCoroutine(EnemyDrop());
        
        
//     }

//     IEnumerator EnemyDrop(){
//         int enemyCount = 0;

//         while(enemyCount < enemyNumber){
//             int xPos = Random.Range(1,999);
//             int zPos = Random.Range(1,999);

//             Instantiate(enemy, new Vector3(xPos,yPos,zPos), Quaternion.identity);

//             yield return new WaitForSeconds(.0001f);
//             enemyCount++;

//         }

        
//     }

    
// }
