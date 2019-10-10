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

            await Task.WhenAll(new Task[] { classTask });

            return new Character(id, classTask.Result, character.Character.Data.Light);
        }

        public long id;
        public DestinyClass classType;
        public long light;

        public Character(long id, DestinyClassDefinition classDef, long light)
        {
            this.id = id;
            classType = classDef.ClassType;
            this.light = light;
        }
    }
}
