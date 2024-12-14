<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['player_id']) && isset($_POST['treasure']) && isset($_POST['ancient_coral']) && isset($_POST['lost_research']) && isset($_POST['temple_jewel']) && isset($_POST['boat_jewel']) && isset($_POST['old_ice'])) {
    $playerId = $_POST['player_id'];    
    $treasure = intval($_POST['treasure']);
    $ancientCoral = intval($_POST['ancient_coral']);
    $lostResearch = intval($_POST['lost_research']);
    $templeJewel = intval($_POST['temple_jewel']);
    $boatJewel = intval($_POST['boat_jewel']);
    $oldIce = intval($_POST['old_ice']);

    $sql = "UPDATE Achievements 
            SET treasure = :treasure, 
                ancient_coral = :ancient_coral, 
                lost_research = :lost_research, 
                temple_jewel = :temple_jewel, 
                boat_jewel = :boat_jewel, 
                old_ice = :old_ice 
            WHERE player_id = :player_id";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':treasure', $treasure, PDO::PARAM_INT);
    $stmt->bindParam(':ancient_coral', $ancientCoral, PDO::PARAM_INT);
    $stmt->bindParam(':lost_research', $lostResearch, PDO::PARAM_INT);
    $stmt->bindParam(':temple_jewel', $templeJewel, PDO::PARAM_INT);
    $stmt->bindParam(':boat_jewel', $boatJewel, PDO::PARAM_INT);
    $stmt->bindParam(':old_ice', $oldIce, PDO::PARAM_INT);
    $stmt->bindParam(':player_id', $playerId, PDO::PARAM_INT);

    try {
        $stmt->execute();
        echo json_encode(["status" => "success", "message" => "Achievements updated successfully"]);
    } catch (PDOException $e) {
        echo json_encode(["status" => "error", "message" => $e->getMessage()]);
    }

    // Close the connection
    $conn = null;
} else {
    echo json_encode(["status" => "error", "message" => "Invalid data"]);
}
?>