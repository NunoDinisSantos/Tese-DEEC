<?php

// Set the SQLite database file
//$dbFile = "path_to_your_database.sqlite";

$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (isset($_GET['PlayerId'])) {
    $player_id = intval($_GET['PlayerId']);

    if (!$conn) {
        echo "Connection could not be established.<br />";
        die("Error: Unable to connect to SQLite database.");
    }

    $sql = "SELECT * FROM MisteriosAquaticos m 
            INNER JOIN Achievements a ON m.PlayerId = a.PlayerId 
            WHERE m.PlayerId = :player_id";

    $stmt = $conn->prepare($sql);
    $stmt->bindParam(':player_id', $player_id, PDO::PARAM_INT);

    try {
        $stmt->execute();
        $data = $stmt->fetch(PDO::FETCH_ASSOC);
        echo json_encode($data); // Send the data back as JSON
    } catch (PDOException $e) {
        echo "Query failed: " . $e->getMessage();
    }

    // Close the connection
    $conn = null;
} else {
    echo "No player ID provided.";
}
?>