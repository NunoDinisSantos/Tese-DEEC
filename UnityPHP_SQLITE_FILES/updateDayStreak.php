<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['PlayerId'])) {
    $PlayerId = intval($_POST['PlayerId']);    
    $dayStreak = intval($_POST['DayStreak']);
    $lastLogin = $_POST['LastLogin']; // Assuming this is in a compatible SQLite datetime format

    $sql = "UPDATE MisteriosAquaticos 
            SET DayStreak = :dayStreak, 
                LastLogin = :lastLogin 
            WHERE PlayerId = :PlayerId";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':dayStreak', $dayStreak, PDO::PARAM_INT);
    $stmt->bindParam(':lastLogin', $lastLogin, PDO::PARAM_STR);
    $stmt->bindParam(':PlayerId', $PlayerId, PDO::PARAM_INT);

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