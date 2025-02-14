<?php
$zip = new ZipArchive;
$file = 'Publish.zip';  // The uploaded ZIP file
$extractPath = './';  // Extract in the same directory

if ($zip->open($file) === TRUE) {
    $zip->extractTo($extractPath);
    $zip->close();
    echo "✅ Extraction successful!";
} else {
    echo "❌ Failed to extract!";
}
?>
