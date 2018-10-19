using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GiveResponse : MonoBehaviour {

    
    public Image[] blinkingSprites;


    // blinks the clock after longclick effective when flagging  -  called in tile
    public void startBlinking()
    {
        StartCoroutine(FlashSprites(blinkingSprites, 2, 0.1f));
        SoundManager.Instance.flagTrigger();
    }


    /**
         * Coroutine to create a flash effect on all sprite renderers passed in to the function.
         *
         * @param sprites   a sprite renderer array
         * @param numTimes  how many times to flash
         * @param delay     how long in between each flash
         * @param disable   if you want to disable the renderer instead of change alpha
         */
    public IEnumerator FlashSprites(Image[] sprites, int numTimes, float delay, bool disable = false)
    {
        // number of times to loop
        for (int loop = 0; loop < numTimes; loop++)
        {
            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                if (disable)
                {
                    // for disabling
                    sprites[i].enabled = false;
                }
                else {
                    // for changing the alpha
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0.5f);
                }
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);

            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                if (disable)
                {
                    // for disabling
                    sprites[i].enabled = true;
                }
                else {
                    // for changing the alpha
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
                }
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
    }
}
