<?php

return;


require __DIR__ . '/../../../../project/hahaha/hahaha/vendor/autoload.php';



$src_ = realpath(__DIR__ . "/node_modules");
$dst_ = realpath(__DIR__ . "/../..") . "/vendor";
$results = [];

$file_ = \hahahalib\file::Instance();

if(is_dir($dst_))
{
	$file_->Delete_Tree($dst_);
}

mkdir($dst_ , 0777, true);

// $list = [

// ];

// 因為實際套版由前端處理
// 這裡採下載後，手動搬移