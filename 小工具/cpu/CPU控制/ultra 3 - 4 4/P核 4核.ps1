# 允許的核心 (0xF = 二進位 1111 = CPU 0~3)
$mask = 0x0F

Get-Process | ForEach-Object {
    try {
        $_.ProcessorAffinity = $mask
    } catch {}
}

Read-Host "Press Enter to continue..."