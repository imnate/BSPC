# <p align="center">羽球高遠發球輔助系統</p>


### 主要功能
         利用Kinect攝影機掃描羽毛球發球姿勢，利用視覺化方式讓發球者能夠調整發球姿勢
         
### 語言
         C#
         
### 套件
         Kinect API
         
## 相關程式碼

    ### 計算右腳角度
    double jtr = Math.Atan2(jrf.Position.Z - jar.Position.Z, jrf.Position.X - jar.Position.X)*180/3.14;
    
    ### 計算左腳角度
    double jtl = Math.Atan2(jlf.Position.Z - jal.Position.Z, jlf.Position.X - jal.Position.X)*180/3.14;
    
    ### 計算肩膀角度
    double jls = Math.Atan2(jsl.Position.Z - jsr.Position.Z, jsl.Position.X - jsr.Position.X)*180/3.14;
    
    ### 右腳與左腳角度差
    if (Math.Abs(jtr - jtl) >= 35 && Math.Abs(jtr - jtl) <= 65)   
    //右腳與左腳的角度差判斷
    FeedBackJtrl.Text = "腳姿正確";
    else
    FeedBackJtrl.Text = "腳姿錯誤";
    if (Math.Abs(jtl - jls) >= 35 && Math.Abs(jtl - jls) <= 65)  
     //肩膀與左腳的角度差判斷
    FeedBackJs.Text = "肩膀正確";
    else
    FeedBackJs.Text = "肩膀錯誤~";
  
    ### 右手姿勢判斷
    if (jpr.Position.Z - jsr.Position.Z > 0.15)//右手至少往後一定距離
    PostureRArms.Text= "右手正確";
    else
    PostureRArms.Text= "右手錯誤~";
    if (jpl.Position.X - ((jsl.Position.X + jsr.Position.X) / 2 + 4 * (jlf.Position.X - jal.Position.X)) >= -0.03 
    && jpl.Position.X - ((jsl.Position.X + jsr.Position.X) / 2 + 4 * (jlf.Position.X - jal.Position.X)) <= 0.03 
    && jpl.Position.Y < shou.Position.Y && jpl.Position.Y > spin.Position.Y && jsl.Position.Z -jpl.Position.Z >= 0.5)
    //左手是擺在以發球的方向擺在胸前一定距離，所以要加上腳的斜率
    PostureLArms.Text = "左手正確";
    else
    PostureLArms.Text = "左手錯誤";
