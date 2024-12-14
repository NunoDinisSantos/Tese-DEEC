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

    $sql = "UPDATE MisteriosAquaticos 
            SET tutorial = 1  
            WHERE player_id = :player_id";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':player_id', $playerId, PDO::PARAM_INT);

    try {
        $stmt->execute();
        echo json_encode(["status" => "success", "message" => "Tutorial updated successfully"]);
    } catch (PDOException $e) {
        echo json_encode(["status" => "error", "message" => $e->getMessage()]);
    }

    // Close the connection
    $conn = null;
} else {
    echo json_encode(["status" => "error", "message" => "Invalid data"]);
}
?>