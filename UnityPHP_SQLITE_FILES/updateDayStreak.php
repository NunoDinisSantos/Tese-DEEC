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
    $dayStreak = intval($_POST['days_streak']);
    $lastLogin = $_POST['last_login']; // Assuming this is in a compatible SQLite datetime format

    $sql = "UPDATE MisteriosAquaticos 
            SET days_streak = :days_streak, 
                last_login = :last_login 
            WHERE player_id = :player_id";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':days_streak', $dayStreak, PDO::PARAM_INT);
    $stmt->bindParam(':last_login', $lastLogin, PDO::PARAM_STR);
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