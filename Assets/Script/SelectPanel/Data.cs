using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{ 
    public class CharacterInfo
    {
        public string avatar;
        public string name;
        public int index;
    }

    public static CharacterInfo[] GetCharacterList()
    {
        CharacterInfo character1 = new CharacterInfo()
        { avatar = "Img/VirtualGuy", name = "Virtual Guy", index = 1};
        CharacterInfo character2 = new CharacterInfo()
        { avatar = "Img/PinkMan", name = "Pink Man", index = 2 };
        CharacterInfo character3 = new CharacterInfo()
        { avatar = "Img/MaskDude", name = "Mask Dude", index = 3 };
        CharacterInfo character4 = new CharacterInfo()
        { avatar = "Img/NinjaFrog", name = "Ninja Frog", index = 4 };

        CharacterInfo[] list = {character1, character2, character3, character4};
        return list;
    }

}
