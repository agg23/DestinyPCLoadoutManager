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
        public static async Task<Character> BuildCharacter(DestinyCharacterResponse character)
        {
            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;
            var classTask = manifest.LoadClass(character.Character.Data.ClassHash);

            await Task.WhenAll(new Task[] { classTask });

            return new Character(classTask.Result, character.Character.Data.Light);
        }

        public DestinyClass classType;
        public long light;

        public Character(DestinyClassDefinition classDef, long light)
        {
            classType = classDef.ClassType;
            this.light = light;
        }
    }
}
