using System;

public class XMLDyanmoDBParser
{
    public void XML_to_DynamoDB()
    {
        var aws = AWSManager.instance.user;
        var xml = XMLManager.ins.itemDB.user;

        aws.boong = xml.boong;
        aws.current_stage = xml.current_stage;
        aws.heart = xml.heart;
        aws.heart_time = xml.heart_time;
        aws.log_out = xml.log_out;
        aws.move_list = xml.move_list;
        aws.reward_list = xml.reward_list;
        aws.star_list = xml.star_list;

        
        aws.ping_skin_num = xml.ping_skin_num;
        aws.peng_skin_num = xml.peng_skin_num;

        aws.profile_skin_num = xml.profile_skin_num;
        aws.profile_introduction = xml.profile_introduction;
        aws.profile_style_num = xml.profile_style_num;

        aws.mySkinList = xml.mySkinList;
        aws.myStyleList = xml.myStyleList;

        aws.drop_count = xml.drop_count;
        aws.carry_count = xml.carry_count;
        aws.reset_count = xml.reset_count;
        aws.move_count = xml.move_count;
        aws.snow_count = xml.snow_count;
        aws.parfait_done_count = xml.parfait_done_count;
        aws.crack_count = xml.crack_count;

        aws.editor_clear_count = xml.editor_clear_count;
        aws.editor_make_count = xml.editor_make_count;

        aws.boong_count = xml.boong_count;
        aws.heart_count = xml.heart_count;

        aws.skin_count = xml.skin_count;

        aws.playTime = xml.playTime;

        XMLManager.ins.SaveItems();
        AWSManager.instance.Update_UserInfo();
    }
    public void DynamoDB_to_XML()
    {
        var aws = AWSManager.instance.user;
        var xml = XMLManager.ins.itemDB.user;

        xml.boong = aws.boong;
        xml.current_stage = aws.current_stage;
        xml.heart = aws.heart;
        xml.heart_time = aws.heart_time;
        xml.log_out = aws.log_out;
        xml.move_list = aws.move_list;
        xml.reward_list = aws.reward_list;
        xml.star_list = aws.star_list;

        xml.ping_skin_num = aws.ping_skin_num;
        xml.peng_skin_num = aws.peng_skin_num;

        xml.profile_skin_num = aws.profile_skin_num;
        xml.profile_introduction = aws.profile_introduction;
        xml.profile_style_num = aws.profile_style_num;

        xml.mySkinList = aws.mySkinList;
        xml.myStyleList = aws.myStyleList;

        xml.drop_count = aws.drop_count;
        xml.carry_count = aws.carry_count;
        xml.reset_count = aws.reset_count;
        xml.move_count = aws.move_count;
        xml.snow_count = aws.snow_count;
        xml.parfait_done_count = aws.parfait_done_count;
        xml.crack_count = aws.crack_count;

        xml.editor_clear_count = aws.editor_clear_count;
        xml.editor_make_count = aws.editor_make_count;

        xml.boong_count = aws.boong_count;
        xml.heart_count = aws.heart_count;

        xml.skin_count = aws.skin_count;

        xml.playTime = aws.playTime;

        XMLManager.ins.SaveItems();
        AWSManager.instance.Update_UserInfo();
    }

    bool XML_Newer_DB_Time(string xml , string aws)//xml이 더 최신시간으로 저장되었다면 true 반
    {
        DateTime time_xml = DateTime.ParseExact(xml, "yyyyMMddHHmmss", null);
        DateTime time_aws = DateTime.ParseExact(xml, "yyyyMMddHHmmss", null);

        return time_xml.Ticks > time_aws.Ticks ? true : false;
    }
}
