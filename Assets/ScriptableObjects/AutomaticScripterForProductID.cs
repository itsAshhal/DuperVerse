using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]  // This attribute allows the script to run in the editor.
public class AutomaticScripterForProductID : MonoBehaviour
{
    public CardSO[] AllCards;

    void Start()
    {
        // Ensure this only runs in the Editor and not during runtime
        if (!Application.isPlaying)
        {
            AssignProductIDs();
        }
    }

    [ContextMenu("Assign IDs")]
    void AssignProductIDs()
    {
        string[] productIds = new string[]
        {
            "8135724892327", // A-Yak Helicopter
            "8204030640295", // Astro Gnome
            "8035688415399", // Bachelor Bear
            "8005797576871", // BakeSale
            "7968568377511", // Beaver Claw
            "8015853682855", // Bierguardian
            "8020886913191", // Big Dino
            "8015859384487", // Big Knife
            "8157975675047", // Chop
           // "8271000010919", // Daddy Chill  ..
            "8030139875495", // Dracula's Dog
           // "8313349111975", // Duperman  ..
            "8029714677927", // Fearless Manta
           // "8290357117095", // Firehouse  ..
            "8135873659047", // Gatorlord
            "8067560341671", // Gospel
            "8021966127271", // HardStop
            "7996384444583", // Honk
            "8029783687335", // Jarawangadananan
            "8170022371495", // Lunar Lightning
           // "8290356887719", // Mega Karen  ..
            "8162591539367", // Monkey Love
            "8237176422567", // Mundo
            //"8262740246695", // NDRYNK  ..
            //"8289030897831", // Officer Mike Nasty  ..
            //"8290356625575", // Post-Op  ..
            //"8237189038247", // Protein Jake  ..
            "8163213017255", // Reptilian Murphy
            //"8263049281703", // Swifto  ..
            "8205446021287", // Theorem
            "8107594481831", // Trashcannibal
            "8210184732839", // Tumbo
            "8198774096039", // U.F.Otis
            "8144046653607"  // Wumpus
        };

        for (int i = 0; i < AllCards.Length && i < productIds.Length; i++)
        {
            AssignValues(AllCards[i], productIds[i]);
        }

        Debug.Log("Product IDs have been assigned to all cards.");
    }

    public void AssignValues(CardSO card, string productId)
    {
        card.ProductID = productId;
    }
}
