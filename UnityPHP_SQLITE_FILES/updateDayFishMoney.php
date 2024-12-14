<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['player_id']) && isset($_POST['days']) && isset($_POST['fish_caught']) && isset($_POST['coins']) && isset($_POST['credits'])) {
    $playerId = $_POST['player_id'];    
    $days = intval($_POST['days']);
    $fishCaught = intval($_POST['fish_caught']);
    $money = intval($_POST['coins']);
    $credits = intval($_POST['credits']);

    $sql = "UPDATE MisteriosAquaticos 
            SET Days = :days, 
                fish_caught = :fish_caught, 
                coins = :money, 
                credits = :credits  
            WHERE player_id = :player_id";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':days', $days, PDO::PARAM_INT);
    $stmt->bindParam(':fish_caught', $fishCaught, PDO::PARAM_INT);
    $stmt->bindParam(':money', $money, PDO::PARAM_INT);
    $stmt->bindParam(':credits', $credits, PDO::PARAM_INT);
    $stmt->bindParam(':player_id', $playerId, PDO::PARAM_STR);

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