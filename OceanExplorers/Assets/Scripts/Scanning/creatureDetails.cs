using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//stores the data abount fish and adds to total collected
public class creatureDetails : MonoBehaviour
{
    public string[] names = { "Shark", "Crab", "Tuna", "Remora", "Marine Angelfish", "Clownfish", "Stingray", "Anchovy", "Tetra", "Blue Tang", "Guppy" };
    public string[] descriptions = { "We all know a shark. These are one of the longest living fish on planet earth (almost 70 years!). Sharks have no known natural predators (other than occasionally the killer whale, but none of those in this game don't worry).", 
        "Crabs are crustaceans which have a little tail that's usually not visible and are covered in a thick exoskeleton and have claws (snippy snaps). They also make a fun xylophone sound when sneaking and they love money.",
        "Tuna are a saltwater fish and are one of the only species of fish that can maintain a body temperature higher than that of the surrounding water. This makes it an active and agile predator.",
         "Often called a suckerfish, Remora like to cling onto the skin of larger marine animals and usually take to the same habitat as that of it's host.",
         "These bright coloured fish are beautiful and seemingly fearless, as they have been known to swim directly up to divers.",
         "Otherwise known as Anemonefish are always in a form of symbiotic relationship with sea anemones. These anemones are both hard to pronounce for little clownfish and helpful for protecting and giving shelter to clownfish and their offspring. Their reproduction system is very fascinating, go look it up!",
         "Stingrays are lovely little creatures that love to swim around on the ocean floor and sometimes regularly. They have a mouth on their underside which can crush shells to get the goodies on the inside. They can also secrete a venom (*sting*ray) which can be quite harmful.",
         "Small, forager fish with a large number of species in 17 generia. These are usually classified as oily fish and they are often used for pizza or to stink up a kitchen.",
         "Tetras are popular fish to be used for fishkeeping, and that's why a lot of fish can be unrelated to the tetra are known as tetra.",
          "A beautiful fish who can grow up to 30cm and weigh 600g. Love to eat plankton. Is mainly used as a bait fish. Loves to keep swimming and lose it's memory.",
          "Also known as the millionfish and the rainbow fish. Is one of the worlds most popular species."};
    public int totalScanned = 0;
    public GameObject[] creatures;
    
    public void increaseScanned()
    {
        totalScanned++;
    }
}
