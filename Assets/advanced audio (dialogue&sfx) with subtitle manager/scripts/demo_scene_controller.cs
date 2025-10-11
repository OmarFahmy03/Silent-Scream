// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// namespace audio_subtitle_system
// {
// public class demo_scene_controller : MonoBehaviour
// {
//     public audio_subtitle_manager asm;
//     public audio_subtitle_player asp;
//     public void play_dialogue1()
//     {
//         asm.subtitle_active=true;
//         asp.display_mode=1;
//         asm.play_audio("dialogue1");
//     }
//     public void play_dialogue2()
//     {
//         asm.subtitle_active=true;
//         asp.display_mode=2;
//         asm.play_audio("dialogue2");
//     }
//     public void play_dialogue3()
//     {
//         asm.subtitle_active=false;
//         asm.play_audio("dialogue3");
//     }
//     public void play_sfx1()
//     {
//         asm.subtitle_active=true;
//         asp.display_mode=1;
//         asm.play_audio("gun shot");
//     }
//     public void play_sfx2()
//     {
//         asm.subtitle_active=false;
//         asm.play_audio("door closing");
//     }
// }
// }