using System.Collections;
using UnityEngine;
using TMPro;

namespace audio_subtitle_system
{
    public class audio_subtitle_player : MonoBehaviour
    {
        // Letters builder vars
        private int alpha_change_factor = 1;
        private int timer;
        public int time_between_letters = 1;
        private int current_letter_index = 0;
        private bool stop = true;
        private string building_string;
        
        // Base vars
        public int display_mode = 1; 
        public string subtitle = "";
        public TextMeshProUGUI text_display;
        public CanvasGroup text_canvas;

        private Coroutine currentDisplayCoroutine;

        public void display(string subtitle_text, float duration)
        {
            // Stop any currently running display
            if (currentDisplayCoroutine != null)
            {
                StopCoroutine(currentDisplayCoroutine);
            }

            subtitle = subtitle_text;
            switch (display_mode) 
            {
                case 1: 
                {
                    currentDisplayCoroutine = StartCoroutine(timed_display_normal(duration));
                    break;
                }
                case 2:
                {
                    currentDisplayCoroutine = StartCoroutine(timed_display_gradual(duration));
                    break;
                }
                default:
                    break;
            }
        }

        IEnumerator timed_display_normal(float duration)
        {
            // Fade in
            text_display.text = subtitle;
            alpha_change_factor = 1;
            text_canvas.alpha = 0f;
            
            // Fade in over 0.3 seconds
            float fadeInTime = Mathf.Min(0.3f, duration * 0.2f);
            float elapsed = 0f;
            while (elapsed < fadeInTime)
            {
                text_canvas.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            text_canvas.alpha = 1f;

            // Hold
            float holdTime = duration - fadeInTime - 0.3f;
            if (holdTime > 0)
            {
                yield return new WaitForSeconds(holdTime);
            }

            // Fade out
            alpha_change_factor = -1;
            elapsed = 0f;
            while (elapsed < 0.3f)
            {
                text_canvas.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            text_canvas.alpha = 0f;

            subtitle = "";
            text_display.text = subtitle;
            currentDisplayCoroutine = null;
        }

        IEnumerator timed_display_gradual(float duration)
        {
            // Setup
            time_between_letters = (int)Mathf.Floor((duration * 50 * 0.8f) / subtitle.Length);
            building_string = "";
            current_letter_index = 0;
            stop = false;
            text_canvas.alpha = 1f; // Visible immediately for gradual mode
            alpha_change_factor = 1;

            // Hold
            if (duration > 0.5f)
            {
                yield return new WaitForSeconds(duration - 0.5f);
                alpha_change_factor = -1;
                
                // Fade out
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    text_canvas.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            text_canvas.alpha = 0f;
            stop = true;
            subtitle = "";
            text_display.text = subtitle;
            currentDisplayCoroutine = null;
        }

        private void FixedUpdate() 
        {
            // Gradual letter building
            if (!stop && subtitle.Length > 0)
            {
                timer++;
                if (timer >= time_between_letters)
                {
                    if (current_letter_index < subtitle.Length)
                    {
                        building_string += subtitle[current_letter_index];
                        text_display.text = building_string;
                        current_letter_index++;
                    }
                    
                    timer = 0;
                    
                    if (current_letter_index >= subtitle.Length)
                    {
                        stop = true;
                    }
                }
            }
        }
    }
}