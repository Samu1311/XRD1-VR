using UnityEngine;

[CreateAssetMenu(fileName = "GreekMythDatabase", menuName = "Greek Scene/Myth Database")]
public class GreekMythDatabase : ScriptableObject
{
    [System.Serializable]
    public class Myth
    {
        public string title;
        [TextArea(3, 10)]
        public string description;
    }

    public Myth[] myths = new Myth[]
    {
        new Myth
        {
            title = "The Twelve Labors of Heracles",
            description = "Heracles, the greatest Greek hero, was forced to perform twelve impossible tasks to atone for killing his family in a fit of madness caused by Hera. These labors included slaying the Nemean Lion, capturing the Golden Hind of Artemis, and retrieving the Golden Apples of the Hesperides."
        },
        new Myth
        {
            title = "Pandora's Box",
            description = "Pandora was the first woman created by the gods. She was given a jar (mistranslated as 'box') and told never to open it. Overcome by curiosity, she opened it, releasing all the evils into the world—disease, death, and suffering. Only Hope remained inside, giving humanity the strength to endure."
        },
        new Myth
        {
            title = "The Fall of Icarus",
            description = "Daedalus crafted wings of feathers and wax for himself and his son Icarus to escape imprisonment. He warned Icarus not to fly too close to the sun. But Icarus, overcome with the thrill of flight, soared too high. The sun melted the wax, and he fell into the sea."
        },
        new Myth
        {
            title = "Perseus and Medusa",
            description = "Perseus was tasked with slaying Medusa, the Gorgon whose gaze turned men to stone. With a mirrored shield from Athena and winged sandals from Hermes, he beheaded Medusa while she slept, using her reflection to avoid her deadly stare."
        },
        new Myth
        {
            title = "The Trojan Horse",
            description = "After ten years of siege, the Greeks built a giant wooden horse and hid their best warriors inside. They pretended to sail away, leaving the horse as a gift. The Trojans brought it into their city, and at night, the Greek soldiers emerged and opened the gates, conquering Troy."
        },
        new Myth
        {
            title = "Orpheus and Eurydice",
            description = "Orpheus, the legendary musician, descended into the Underworld to rescue his wife Eurydice from death. His music moved Hades, who agreed to let her return—on one condition: Orpheus must not look back. But at the last moment, he turned, and she vanished forever."
        },
        new Myth
        {
            title = "King Midas and the Golden Touch",
            description = "King Midas was granted one wish by Dionysus and asked that everything he touched turn to gold. At first delighted, he soon realized his curse when his food, drink, and even his daughter turned to gold. He begged Dionysus to take back the gift."
        },
        new Myth
        {
            title = "The Minotaur and the Labyrinth",
            description = "King Minos imprisoned the monstrous Minotaur—half man, half bull—in a vast labyrinth designed by Daedalus. Every year, Athens sent seven youths as tribute. The hero Theseus volunteered, navigated the maze with Ariadne's thread, and slew the beast."
        },
        new Myth
        {
            title = "Prometheus Steals Fire",
            description = "Prometheus, a Titan, defied Zeus by stealing fire from the gods and giving it to humanity. Enraged, Zeus punished him by chaining him to a rock where an eagle would eat his liver daily—only for it to regenerate each night, repeating his torment eternally."
        },
        new Myth
        {
            title = "The Odyssey of Odysseus",
            description = "After the fall of Troy, Odysseus spent ten years trying to return home. He faced the Cyclops, resisted the Sirens' song, navigated between Scylla and Charybdis, and was held captive by Calypso. His cleverness and determination finally brought him back to Ithaca."
        }
    };

    public string GetMythText(int index)
    {
        if (index >= 0 && index < myths.Length)
        {
            return $"{myths[index].title}\n\n{myths[index].description}";
        }
        return "Ancient Greek myth lost to time...";
    }
}
