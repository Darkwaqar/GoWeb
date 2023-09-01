<?php
include("class.phpmailer.php"); //you have to upload class files "class.phpmailer.php" and "class.smtp.php"
$email = $_POST['email'];
$mail = new PHPMailer();
$mail->IsSMTP();
$mail->SMTPAuth = true;
$mail->Host = "mail.sadafamir.com";
$mail->Username = "info@sadafamir.com";
$mail->Password = "info_1234"; 
$mail->From = "info@sadafamir.com";
$mail->FromName = "Sadaf Amir";
$mail->AddAddress($email,"Subscriber");
$mail->Subject = "From: Sadaf Amir";
$mail->Body = "Congratulations

Thank You for subscribing to Sadaf Amir

You will be notified when Sadaf Amir is live so you can shop the exclusive collection so you can shop her exclusive collection online";
$mail->WordWrap = 50;
$mail->IsHTML(true);
$str1= "gmail.com";
$str2=strtolower("mail.sadafamir.com");
If(strstr($str2,$str1))
{
$mail->SMTPSecure = 'tls';
$mail->Port = 587;
if(!$mail->Send()) {
echo "Mailer Error: " . $mail->ErrorInfo;
echo "<br><br> * Please double check the user name and password to confirm that both of them are correct. <br><br>";
echo "* If you are the first time to use gmail smtp to send email, please refer to this link :http://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx?KBSearchID=137388";
} 
else {
echo "<h1 style='text-align: center;font-family: Open Sans;width: 97%;position: absolute;top: 50%;margin: 0;text-transform: uppercase;font-weight: 400;padding: 0 20px;'>
	You have subscribed to Sadaf Amir</h1>";
}
}
else{
	$mail->Port = 25;
	if(!$mail->Send()) {
echo "Mailer Error: " . $mail->ErrorInfo;
echo "<br><BR>* Please double check the user name and password to confirm that both of them are correct. <br>";
} 
else {
echo "<h1 style='text-align: center;font-family: Open Sans;width: 97%;position: absolute;top: 50%;margin: 0;text-transform: uppercase;font-weight: 400;padding: 0 20px;'>
	You have subscribed to Sadaf Amir</h1>";
}
}


$mail1 = new PHPMailer();
$mail1->IsSMTP();
$mail1->SMTPAuth = true;
$mail1->Host = "mail.sadafamir.com";
$mail1->Username = "info@sadafamir.com";
$mail1->Password = "info_1234"; 
$mail1->From = "info@sadafamir.com";
$mail1->FromName = "Sadaf Amir";
$mail1->AddAddress("info@sadafamir.com","info");
$mail1->Subject = "$email has subscribed to Sadaf Amir";
$mail1->Body = " ";
$mail1->WordWrap = 50;
$mail1->IsHTML(true);
$str1= "gmail.com";
$str2=strtolower("mail.sadafamir.com");
If(strstr($str2,$str1))
{
$mail1->SMTPSecure = 'tls';
$mail1->Port = 587;
if(!$mail1->Send()) {
} 
else {
}
}
else{
	$mail1->Port = 25;
	if(!$mail1->Send()) {
echo "Mailer Error: " . $mail->ErrorInfo;
echo "<br><BR>* Please double check the user name and password to confirm that both of them are correct. <br>";
} 
else {
}
}