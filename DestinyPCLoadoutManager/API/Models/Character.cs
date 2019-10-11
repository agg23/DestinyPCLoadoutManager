using Destiny2;
using Destiny2.Definitions;
using Destiny2.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API.Models
{
    public class Character
    {
        public static async Task<Character> BuildCharacter(long id, DestinyCharacterResponse character)
        {
            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;
            var classTask = manifest.LoadClass(character.Character.Data.ClassHash);
            var inventoryTask = Inventory.BuildCharacterInventory(character);

            await Task.WhenAll(new Task[] { classTask, inventoryTask });

            return new Character(id, classTask.Result, character.Character.Data.Light, inventoryTask.Result);
        }

        public long Id;
        public DestinyClass ClassType;
        public long Light;
        public Inventory Inventory;

        public Character(long id, DestinyClassDefinition classDef, long light, Inventory Inventory)
        {
            Id = id;
            ClassType = classDef.ClassType;
            Light = light;
            this.Inventory = Inventory;
        }

        public async Task UpdateInventory(DestinyCharacterResponse character)
        {
            Inventory = await Inventory.BuildCharacterInventory(character);
        }
    }
}
