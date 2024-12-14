<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['player_id'])) {
    $playerId = $_POST['player_id'];    
    $deepModule = intval($_POST['depth_module']);
    $tempModule = intval($_POST['temp_module']);
    $reelModule = intval($_POST['reel_module']);
    $storageModule = intval($_POST['storage_module']);
    $flashlight = intval($_POST['flashlight']);
    $coins = intval($_POST['coins']);

    $sql = "UPDATE MisteriosAquaticos 
            SET depth_module = :depth_module, 
                temp_module = :temp_module, 
                reel_module = :reel_module, 
                storage_module = :storage_module, 
                flashlight = :flashlight,
                coins = :coins 
            WHERE player_id = :player_id";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':depth_module', $deepModule, PDO::PARAM_INT);
    $stmt->bindParam(':temp_module', $tempModule, PDO::PARAM_INT);
    $stmt->bindParam(':reel_module', $reelModule, PDO::PARAM_INT);
    $stmt->bindParam(':coins', $coins, PDO::PARAM_INT);
    $stmt->bindParam(':storage_module', $storageModule, PDO::PARAM_INT);
    $stmt->bindParam(':flashlight', $flashlight, PDO::PARAM_INT);
    $stmt->bindParam(':player_id', $playerId, PDO::PARAM_INT);

    try {
        $stmt->execute();
        echo json_encode(["status" => "success", "message" => "Data updated successfully"]);
    } catch (PDOException $e) {
        echo json_encode(["status" => "error", "message" => $e->getMessage()]);
    }

    // Close the connection
    $conn = null;
} else {
    echo json_encode(["status" => "error", "message" => "Invalid data"]);
}
?>