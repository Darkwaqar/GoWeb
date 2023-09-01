<?php
$email = $_POST['ContactEmail'];
$ContactName = $_POST['ContactName'];
$ContactNumber = $_POST['ContactNumber'];

$to = "contact@gomobishop.com";
$subject = $email. " Want to become vendor on Paranda";
$txt = "$email Want to become vendor on Paranda Contact him on : $ContactName  and Name is : $ContactName ";
$headers = "From: " .$email;

mail($to,$subject,$txt,$headers);
header("Location: /index.cshtml");
?>
