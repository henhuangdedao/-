using UnityEngine;

namespace Utility
{
    public class ScreenHelper
    {
        public static Vector2 LeftBottomScreenPos=new Vector2(-17.8f,-10);
        public static Vector2 RightTopScreenPos=new Vector2(17.8f,10);
        public static float ScreenWorldWidth => RightTopScreenPos.x - LeftBottomScreenPos.x;
        public static float ScreenWorldHeight => RightTopScreenPos.y - LeftBottomScreenPos.y;
        public static void RepeatScreen(Transform transform, float offsetX, float offsetY)
        {
           
            
       
        
            //超出屏幕左边则移到屏幕右边
            if (transform.position.x < LeftBottomScreenPos.x-offsetX)
            {
                transform.position=new Vector3(transform.position.x+ScreenWorldWidth+offsetX*2,transform.position.y,transform.position.z);
            }

            //超出屏幕右边则移到屏幕左边
            else if (transform.position.x > RightTopScreenPos.x+offsetX)
            {
                transform.position=new Vector3(transform.position.x-ScreenWorldWidth-offsetX*2,transform.position.y,transform.position.z);
            }
            //超出屏幕上边则移到屏幕下边
            if (transform.position.y > RightTopScreenPos.y+offsetY)
            {
                transform.position=new Vector3(transform.position.x,transform.position.y-ScreenWorldHeight-offsetY*2,transform.position.z);
            }
            //超出屏幕下边则移到屏幕上边
            else if (transform.position.y < LeftBottomScreenPos.y-offsetY)
            {
                transform.position=new Vector3(transform.position.x,transform.position.y+ScreenWorldHeight+offsetY*2,transform.position.z);
            }
        }
    }
}