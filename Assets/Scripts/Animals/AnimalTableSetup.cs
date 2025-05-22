using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalTable : MonoBehaviour
{
    [SerializeField] private AnimalSelector[] tables;
    [SerializeField] private AnimalCard cardPrefab;
    [SerializeField] private Animal[] allAnimals;
    [SerializeField] private int animalCount = 12;

    private void Start()
    {
        if (allAnimals == null || allAnimals.Length == 0)
        {
            allAnimals = Resources.LoadAll<Animal>("Animals");
        }

        // Shuffle and assign animals
        var randomAnimals = allAnimals.OrderBy(_ => Random.value).Take(animalCount).ToList();

        foreach (AnimalSelector table in tables)
        {
            var cards = table.GetComponentsInChildren<AnimalCard>().ToList();

            // Remove if extra cards
            while (cards.Count > animalCount)
            {
                var card = cards[cards.Count - 1];
                Destroy(card.gameObject);
                cards.RemoveAt(cards.Count - 1);
            }

            // Add if missing cards
            while (cards.Count < animalCount)
            {
                cards.Add(Instantiate(cardPrefab, table.transform));
            }


            for (int i = 0; i < animalCount; i++)
            {
                cards[i].Animal = randomAnimals[i];
            }
        }

        foreach (AnimalSelector table in tables)
        {
            Animal randomAnimal = randomAnimals[Random.Range(0, randomAnimals.Count)];
            table.ThisAnimal = randomAnimal;
            table.myAnimalDisplay.Animal = randomAnimal;
        }

    }
}
